// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using NetcdfSharp.java.io;
using NetcdfSharp.java.util;

/*
 * Copyright (c) 1998-2018 John Caron and University Corporation for Atmospheric Research/Unidata
 * See LICENSE for license information.
 */
namespace NetcdfSharp.ucar.nc2.util
{
	using CalendarDate = NetcdfSharp.ucar.nc2.time.CalendarDate;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// Manages a place on disk to persistently cache files, which are deleted when the last modified date exceeds a certain
	/// time.
	/// This starts up a thread to periodically scour itself; be sure to call exit() to terminate the thread.
	/// 
	/// <para>
	/// Each DiskCache has a "root directory", which may be set as an absolute path, or relative to the
	/// DiskCache "home directory". The root directory must be writeable.
	/// 
	/// The DiskCache home directory is set in the following order:
	/// <ol>
	/// <li>through the system property "nj22.cache" if it exists
	/// <li>through the system property "user.home" if it exists
	/// <li>through the system property "user.dir" if it exists
	/// <li>to the current working directory
	/// </ol>
	/// 
	/// TODO will move in ver 6
	/// </para>
	/// </summary>
	public class DiskCache2
	{
	  private static readonly NetcdfSharp.org.slf4j.Logger cacheLog = NetcdfSharp.org.slf4j.LoggerFactory.getLogger("cacheLogger");
	  private const string lockExtension = ".reserve";
	  private static Timer timer;

	  /// <summary>
	  /// Be sure to call this when your application exits, otherwise your process may not exit without being killed. </summary>
	  public static void exit()
	  {
		if (timer != null)
		{
		  timer.cancel();
		  cacheLog.info("DiskCache2.exit()");
		}
		timer = null;
	  }

	  private static void startTimer()
	  {
		  lock (typeof(DiskCache2))
		  {
			if (timer == null)
			{
			  timer = new Timer("DiskCache2");
			}
		  }
	  }

	  /////////////////////////////////////////////////////////////

	  public enum CachePathPolicy
	  {
		OneDirectory,
		NestedDirectory,
		NestedTruncate
	  }

	  private CachePathPolicy cachePathPolicy = CachePathPolicy.NestedDirectory;
	  private bool alwaysUseCache;
	  private bool neverUseCache;
	  private string cachePathPolicyParam;

	  private string root;
	  private int persistMinutes, scourEveryMinutes;
	  private bool fail;

	  /// <summary>
	  /// Default DiskCache2 strategy: use $user_home/.unidata/cache/, no scouring, alwaysUseCache = false
	  /// Mimics default DiskCache static class
	  /// </summary>
	  public static DiskCache2 Default
	  {
		  get
		  {
			string root = JSystem.getProperty("nj22.cache");

			if (string.ReferenceEquals(root, null))
			{
			  string home = JSystem.getProperty("user.home");

			  if (string.ReferenceEquals(home, null))
			  {
				home = JSystem.getProperty("user.dir");
			  }

			  if (string.ReferenceEquals(home, null))
			  {
				home = ".";
			  }

			  root = home + "/.unidata/cache/";
			}

			DiskCache2 result = new DiskCache2();
			result.RootDirectory = root;
			result.alwaysUseCache = false;
			return result;
		  }
	  }

	  // NOOP
	  public static DiskCache2 Noop
	  {
		  get
		  {
			DiskCache2 noop = new DiskCache2();
			noop.neverUseCache = true;
			return noop;
		  }
	  }

	  private DiskCache2()
	  {
	  }

	  /// <summary>
	  /// Create a cache on disk. Use default policy (CachePathPolicy.NestedDirectory)
	  /// </summary>
	  /// <param name="root"> the root directory of the cache. Must be writeable. </param>
	  /// <param name="relativeToHome"> if the root directory is relative to the cache home directory. </param>
	  /// <param name="persistMinutes"> a file is deleted if its last modified time is greater than persistMinutes </param>
	  /// <param name="scourEveryMinutes"> how often to run the scour process. If <= 0, don't scour. </param>
	  public DiskCache2(string root, bool relativeToHome, int persistMinutes, int scourEveryMinutes)
	  {
		this.persistMinutes = persistMinutes;
		this.scourEveryMinutes = scourEveryMinutes;

		if (relativeToHome)
		{
		  string home = JSystem.getProperty("nj22.cachePersistRoot");

		  if (string.ReferenceEquals(home, null))
		  {
			home = JSystem.getProperty("user.home");
		  }

		  if (string.ReferenceEquals(home, null))
		  {
			home = JSystem.getProperty("user.dir");
		  }

		  if (string.ReferenceEquals(home, null))
		  {
			home = ".";
		  }

		  if (!home.EndsWith("/", StringComparison.Ordinal))
		  {
			home = home + "/";
		  }

		  root = home + root;
		}
		RootDirectory = root;

		if (!fail && scourEveryMinutes > 0)
		{
		  startTimer();
		  DateTime c = new DateTime(); // contains current startup time
		  c = c.AddMinutes(scourEveryMinutes);
		  timer.scheduleAtFixedRate(new CacheScourTask(this), c, (long) 1000 * 60 * scourEveryMinutes);
		  cacheLog.info("Started a DiskCache2 scour task on {} every {} minutes for files older than {} minutes", root, scourEveryMinutes, persistMinutes);
		}

	  }

	  /*
	   * Set the cache root directory. Create it if it doesnt exist.
	   * Normally this is set in the Constructor.
	   * 
	   * @param cacheDir the cache directory
	   */
	  public virtual string RootDirectory
	  {
		  set
		  {
			if (!value.EndsWith("/", StringComparison.Ordinal))
			{
			  value = value + "/";
			}
			root = StringUtil2.replace(value, '\\', "/"); // no nasty backslash

			File dir = new File(root);
			if (!dir.mkdirs())
			{
			  if (!dir.exists())
			  {
				cacheLog.warn("Failed to create DiskCache2 root at {}. Reason unknown.", dir);
			  }
			}
			else
			{
			  cacheLog.debug("DiskCache2 root created at {}.", dir);
			}

			if (!dir.exists())
			{
			  fail = true;
			  cacheLog.error("DiskCache2 failed to create directory " + root);
			}
			else
			{
			  cacheLog.debug("DiskCache2 create directory " + root);
			}
		  }
		  get
		  {
			return root;
		  }
	  }


	  /// <summary>
	  /// Get a File in the cache, corresponding to the fileLocation.
	  /// File may or may not exist.
	  /// If fileLocation has "/" in it, and cachePathPolicy == NestedDirectory, the
	  /// nested directories will be created.
	  /// </summary>
	  /// <param name="fileLocation"> logical file location </param>
	  /// <returns> physical File in the cache. </returns>
	  public virtual File getCacheFile(string fileLocation)
	  {
		if (neverUseCache)
		{
		  return null;
		}

		if (!alwaysUseCache)
		{
		  File f = new File(fileLocation);
		  if (canWrite(f))
		  {
			return f;
		  }
		}

		File f = new File(makeCachePath(fileLocation));
		// if (f.exists())
		// f.setLastModified( System.currentTimeMillis());

		if (cachePathPolicy == CachePathPolicy.NestedDirectory)
		{
		  File dir = f.getParentFile();
		  if (!dir.exists())
		  {
			bool ret = dir.mkdirs();
			if (!ret)
			{
			  cacheLog.warn("Error creating dir: " + dir);
			}
		  }
		}

		return f;
	  }

	  /// <summary>
	  /// Get the named File.
	  /// If exists or isWritable, return it.
	  /// Otherwise get corresponding file in the cache directory.
	  /// 
	  /// If fileLocation has "/" in it, and cachePathPolicy == NestedDirectory, the
	  /// nested directories will be created.
	  /// </summary>
	  /// <param name="fileLocation"> logical file location </param>
	  /// <returns> physical File as named, or in the cache. </returns>
	  public virtual File getFile(string fileLocation)
	  {
		if (!alwaysUseCache)
		{
		  File f = new File(fileLocation);
		  if (f.exists())
		  {
			return f;
		  }
		  if (canWrite(f))
		  {
			return f;
		  }
		}

		if (neverUseCache)
		{
		  throw new System.InvalidOperationException("neverUseCache=true, but file does not exist and directory is not writeable =" + fileLocation);
		}

		File f = new File(makeCachePath(fileLocation));
		if (cachePathPolicy == CachePathPolicy.NestedDirectory)
		{
		  File dir = f.getParentFile();
		  if (!dir.exists() && !dir.mkdirs())
		  {
			cacheLog.warn("Cant create directories for file " + dir.getPath());
		  }
		}

		return f;
	  }

	  /// <summary>
	  /// Returns {@code true} if we can write to the file.
	  /// </summary>
	  /// <param name="f"> a file. It may be a regular file or a directory. It may even be non-existent, in which case the
	  ///        writability of the file's parent dir is tested. </param>
	  /// <returns> {@code true} if we can write to the file. </returns>
	  public static bool canWrite(File f)
	  {
		try
		{
		  string path = f.GetAbsolutePath();
		  if (Directory.Exists(path))
		  {
			string tmp = System.IO.Path.Combine(path, "check_" + Guid.NewGuid().ToString("N"));
			using (System.IO.File.Create(tmp)) { }
			System.IO.File.Delete(tmp);
		  }
		  else if (System.IO.File.Exists(path))
		  {
			using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write)) { }
		  }
		  else
		  {
			string parent = System.IO.Path.GetDirectoryName(path);
			string tmp = System.IO.Path.Combine(parent, "check_" + Guid.NewGuid().ToString("N"));
			using (System.IO.File.Create(tmp)) { }
			System.IO.File.Delete(tmp);
		  }
		}
		catch (Exception e) when (e is IOException || e is System.Security.SecurityException)
		{
		  return false;
		}

		return true;
	  }

	  /// <summary>
	  /// Looking for an existing file, in cache or no
	  /// </summary>
	  /// <param name="fileLocation"> the original name </param>
	  /// <returns> existing file if you can find it </returns>
	  public virtual File getExistingFileOrCache(string fileLocation)
	  {
		File f = new File(fileLocation);
		if (f.exists())
		{
		  return f;
		}

		if (neverUseCache)
		{
		  return null;
		}

		File fc = new File(makeCachePath(fileLocation));
		if (fc.exists())
		{
		  return fc;
		}

		return null;
	  }

	  /// <summary>
	  /// Reserve a new, uniquely named file in the root directory.
	  /// Mimics File.createTempFile(), but does not actually create
	  /// the file. Instead, a 0 byte file of the same name with the
	  /// extension lock is created, thus "reserving" the name until
	  /// it is used. The filename must be used before persistMinutes,
	  /// or the lock will be scoured and the reservation will be lost.
	  /// </summary>
	  /// <param name="prefix"> The prefix string to be used in generating the file's
	  ///        name; must be at least three characters long </param>
	  /// <param name="suffix"> The suffix string to be used in generating the file's
	  ///        name; may be <code>null</code>, in which case the
	  ///        suffix <code>".tmp"</code> will be used </param>
	  /// <returns> unique, reserved file name </returns>
	  public virtual File createUniqueFile(string prefix, string suffix)
	  {
		  lock (this)
		  {
			if (string.ReferenceEquals(suffix, null))
			{
			  suffix = ".tmp";
			}

			Random random = new Random(DateTimeHelper.CurrentUnixTimeMillis());
			string lockName = prefix + random.Next() + suffix + lockExtension;
			File @lock = new File(RootDirectory, lockName);
			while (@lock.exists())
			{
			  lockName = prefix + random.Next() + suffix + lockExtension;
			  @lock = new File(RootDirectory, lockName);
			}

			// create a lock file to reserve the name
			try
			{
			  @lock.createNewFile();
			  cacheLog.debug(string.Format("Reserved filename {0} for future use.", lockName.Replace(lockExtension, "")));
			}
			catch (IOException)
			{
			  // should not happen, as DiskCache2 had to make the directory before we can even get here
			  // ...but just in case...
			  cacheLog.error(string.Format("Error creating lock file: {0}. May result in cache file collisions.", @lock));
			}

			// reserved name will be the lock file name, minus the lock extension
			return new File(RootDirectory, @lock.getName().replace(lockExtension, ""));
		  }
	  }

	  /// <summary>
	  /// Set the cache path policy
	  /// </summary>
	  /// <param name="cachePathPolicy"> one of:
	  ///        OneDirectory (default) : replace "/" with "-", so all files are in one directory.
	  ///        NestedDirectory: cache files are in nested directories under the root.
	  ///        NestedTruncate: eliminate leading directories </param>
	  public virtual void setPolicy(CachePathPolicy cachePathPolicy)
	  {
		this.cachePathPolicy = cachePathPolicy;
	  }

	  /// <summary>
	  /// Set the cache path policy
	  /// </summary>
	  /// <param name="cachePathPolicy"> one of:
	  ///        OneDirectory (default) : replace "/" with "-", so all files are in one directory.
	  ///        NestedDirectory: cache files are in nested directories under the root.
	  ///        NestedTruncate: eliminate leading directories
	  /// </param>
	  /// <param name="cachePathPolicyParam"> for NestedTruncate, eliminate this string </param>
	  public virtual void setCachePathPolicy(CachePathPolicy cachePathPolicy, string cachePathPolicyParam)
	  {
		this.cachePathPolicy = cachePathPolicy;
		this.cachePathPolicyParam = cachePathPolicyParam;
	  }

	  public virtual void setPolicy(string policy)
	  {
		if (string.ReferenceEquals(policy, null))
		{
		  return;
		}
		if (policy.Equals("oneDirectory", StringComparison.OrdinalIgnoreCase))
		{
		  setCachePathPolicy(CachePathPolicy.OneDirectory, null);
		}
		else if (policy.Equals("nestedDirectory", StringComparison.OrdinalIgnoreCase))
		{
		  setCachePathPolicy(CachePathPolicy.NestedDirectory, null);
		}
	  }

	  /// <summary>
	  /// If true, always put the file in the cache. default false.
	  /// </summary>
	  /// <param name="alwaysUseCache"> If true, always put the file in the cache </param>
	  public virtual bool AlwaysUseCache
	  {
		  set
		  {
			this.alwaysUseCache = value;
		  }
	  }


	  /// <summary>
	  /// If true, never put the file in the cache. default false.
	  /// </summary>
	  /// <param name="neverUseCache"> If true, never put the file in the cache </param>
	  public virtual bool NeverUseCache
	  {
		  set
		  {
			this.neverUseCache = value;
		  }
	  }

	  /// <summary>
	  /// Make the cache filename
	  /// </summary>
	  /// <param name="fileLocation"> normal file location </param>
	  /// <returns> cache filename </returns>
	  private string makeCachePath(string fileLocation)
	  {

		// remove ':', '?', '=', replace '\' with '/', leading or trailing '/'
		string cachePath = fileLocation;
		cachePath = StringUtil2.remove(cachePath, '?');
		cachePath = StringUtil2.remove(cachePath, '=');
		cachePath = StringUtil2.replace(cachePath, '\\', "/");
		if (cachePath.StartsWith("/", StringComparison.Ordinal))
		{
		  cachePath = cachePath.Substring(1);
		}
		if (cachePath.EndsWith("/", StringComparison.Ordinal))
		{
		  cachePath = cachePath.Substring(0, cachePath.Length - 1);
		}
		cachePath = StringUtil2.remove(cachePath, ':');

		// remove directories
		if (cachePathPolicy == CachePathPolicy.OneDirectory)
		{
		  cachePath = StringUtil2.replace(cachePath, '/', "-");
		}

		// eliminate leading directories
		else if (cachePathPolicy == CachePathPolicy.NestedTruncate)
		{
		  int pos = cachePath.IndexOf(cachePathPolicyParam, StringComparison.Ordinal);
		  if (pos >= 0)
		  {
			cachePath = cachePath.Substring(pos + cachePathPolicyParam.Length);
		  }
		  if (cachePath.StartsWith("/", StringComparison.Ordinal))
		  {
			cachePath = cachePath.Substring(1);
		  }
		}

		// make sure the parent directory exists
		if (cachePathPolicy != CachePathPolicy.OneDirectory)
		{
		  File file = new File(root + cachePath);
		  File parent = file.getParentFile();
		  if (!parent.exists())
		  {
			if (string.ReferenceEquals(root, null))
			{ // LOOK shouldnt happen, remove soon
			  cacheLog.error("makeCachePath has null root %{}", parent.getPath());
			  Console.WriteLine((new Exception()).ToString());
			  Console.Write((new Exception()).StackTrace);
			}
			bool ret = parent.mkdirs();
			if (!ret)
			{
			  cacheLog.warn("Error creating parent: " + parent);
			}
		  }
		}

		return root + cachePath;
	  }

	  /// <summary>
	  /// Show cache contents, for debugging.
	  /// </summary>
	  /// <param name="pw"> write to this PrintStream. </param>
	  public virtual void showCache(PrintStream pw)
	  {
		pw.println("Cache files");
		pw.println("Size   LastModified       Filename");
		File dir = new File(root);
		File[] files = dir.listFiles();
		if (files != null)
		{
		  foreach (File file in files)
		  {
			string org = null;
			try
			{
			  org = URLDecoder.decode(file.getName(), "UTF8");
			}
			catch (UnsupportedEncodingException e)
			{
			  Console.WriteLine(e.ToString());
			  Console.Write(e.StackTrace);
			}

			pw.println(" " + file.length() + " " + new DateTime(file.lastModified()) + " " + org);
		  }
		}
	  }

	  /// <summary>
	  /// Remove any files or directories whose last modified time greater than persistMinutes
	  /// </summary>
	  /// <param name="dir"> clean starting here </param>
	  /// <param name="sbuff"> status messages here, may be null </param>
	  /// <param name="isRoot"> delete empty directories, bit not root directory </param>
	  public virtual void cleanCache(File dir, Formatter sbuff, bool isRoot)
	  {
		long now = DateTimeHelper.CurrentUnixTimeMillis();
		File[] files = dir.listFiles();
		if (files == null)
		{
		  throw new System.InvalidOperationException("DiskCache2: not a directory or I/O error on dir=" + dir.getAbsolutePath());
		}

		// check for empty directory
		if (!isRoot && (files.Length == 0))
		{
		  purgeExpired(dir, sbuff, now);
		  return;
		}

		// check for expired files
		foreach (File file in files)
		{
		  if (file.isDirectory())
		  {
			cleanCache(file, sbuff, false);
		  }
		  else
		  {
			purgeExpired(file, sbuff, now);
		  }
		}
	  }

	  private void purgeExpired(File deletable, Formatter sbuff, long now)
	  {
		long duration = now - deletable.lastModified();
		duration /= 1000 * 60; // minutes
		if (duration > persistMinutes)
		{
		  bool ok = deletable.delete();
		  if (!ok)
		  {
			string type = deletable.isDirectory() ? "directory" : "file";
			cacheLog.error("Unable to delete {} {}", type, deletable.getAbsolutePath());
		  }
		  if (sbuff != null)
		  {
			sbuff.format(" deleted %s %s lastModified= %s%n", ok, deletable.getPath(), CalendarDate.of(deletable.lastModified()));
		  }
		}
	  }

	  private class CacheScourTask : TimerTask
	  {
		  private readonly DiskCache2 outerInstance;

		  public CacheScourTask(DiskCache2 outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public override void run()
		  {
			  Formatter sbuff = new Formatter();
			  sbuff.format("DiskCache2 scour on directory= %s%n", outerInstance.root);
			  outerInstance.cleanCache(new File(outerInstance.root), sbuff, true);
			  if (cacheLog.isDebugEnabled())
			  {
				  cacheLog.debug(sbuff.ToString());
			  }
		  }
	  }

	  public override string ToString()
	  {
		string sb = "DiskCache2" + "{cachePathPolicy=" + cachePathPolicy + ", alwaysUseCache=" + alwaysUseCache
			+ ", cachePathPolicyParam='" + cachePathPolicyParam + '\'' + ", root='" + root + '\'' + ", scourEveryMinutes="
			+ scourEveryMinutes + ", persistMinutes=" + persistMinutes + ", fail=" + fail + '}';
		return sb;
	  }

	}

}
