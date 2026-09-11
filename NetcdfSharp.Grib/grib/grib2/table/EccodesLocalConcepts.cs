// ====================================================================================================
// Produced by the Free Edition of Java to C# Converter.
// Purchase a Premium Edition license at:
// https://www.tangiblesoftwaresolutions.com/order/order-java-to-csharp.html
// ====================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NetcdfSharp.ucar.nc2.grib.grib2.table
{
	using GribTables = NetcdfSharp.ucar.nc2.grib.GribTables;
	using Grib2Parameter = NetcdfSharp.ucar.nc2.grib.grib2.Grib2Parameter;
	using StringUtil2 = NetcdfSharp.ucar.unidata.util.StringUtil2;

	/// <summary>
	/// Parse the localConcept files needed to create tables for use by the CDM.
	/// </summary>
	internal class EccodesLocalConcepts
	{
	  private static readonly Logger logger = LoggerFactory.getLogger(typeof(EccodesLocalTables));
	  private const string DISCIPLINE = "discipline";
	  private const string CATEGORY = "parameterCategory";
	  private const string NUMBER = "parameterNumber";
	  private static readonly Charset ENCODING = Encoding.UTF8;

	  private readonly string tableId;
	  private ImmutableListMultimap<string, LocalConcept> localConcepts;

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: EccodesLocalConcepts(String directoryPath) throws NetcdfSharp.java.io.IOException
	  internal EccodesLocalConcepts(string directoryPath)
	  {
		string[] dirs = System.Text.RegularExpressions.Regex.Split(directoryPath, "/");
		this.tableId = dirs[dirs.Length - 1];

		ImmutableListMultimap<LocalConceptPart, LocalConceptPart>.Builder partsBuilder = ImmutableListMultimap<LocalConceptPart, LocalConceptPart>.builder();
		parseLocalConcept(partsBuilder, directoryPath + "/name.def", Type.name);
		parseLocalConcept(partsBuilder, directoryPath + "/shortName.def", Type.shortName);
		parseLocalConcept(partsBuilder, directoryPath + "/paramId.def", Type.paramId);
		parseLocalConcept(partsBuilder, directoryPath + "/units.def", Type.units);
		parseLocalConcept(partsBuilder, directoryPath + "/cfName.def", Type.cfName);
		parseLocalConcept(partsBuilder, directoryPath + "/cfVarName.def", Type.cfVarName);

		ImmutableListMultimap<string, LocalConcept>.Builder conceptsBuilder = ImmutableListMultimap<string, LocalConcept>.builder();
		foreach (ICollection<LocalConceptPart> parts in partsBuilder.build().asMap().Values)
		{
		  LocalConcept localConcept = null;
		  foreach (LocalConceptPart part in parts)
		  {
			if (localConcept == null)
			{
			  localConcept = new LocalConcept(this, part);
			}
			else
			{
			  localConcept.merge(part);
			}
		  }
		  if (localConcept != null)
		  {
			conceptsBuilder.put(localConcept.Key, localConcept);
		  }
		}
		localConcepts = conceptsBuilder.build();
	  }

	  internal virtual ImmutableListMultimap<int, Grib2Parameter> LocalConceptMultimap
	  {
		  get
		  {
			ImmutableListMultimap<int, Grib2Parameter>.Builder result = ImmutableListMultimap<int, Grib2Parameter>.builder();
			foreach (LocalConcept lc in localConcepts.values())
			{
			  if (!string.ReferenceEquals(lc.Name, null) && !string.ReferenceEquals(lc.ShortName, null))
			  {
				int code = Grib2Tables.makeParamId(lc.discipline, lc.category, lc.number);
				Grib2Parameter param = new Grib2Parameter(lc.discipline, lc.category, lc.number, lc.Name, lc.Units, lc.ShortName, null);
				result.put(code, param);
			  }
			}
			return result.build();
		  }
	  }

	  /*
	   * #paramName
	   * 'value' = {
	   * attName = attValue ;
	   * attName = attValue ;
	   * attName = attValue ;
	   * }
	   * examples:
	   * 
	   * #Potential vorticity
	   * '82001004' = {
	   * discipline = 0 ;
	   * parameterCategory = 2 ;
	   * parameterNumber = 14 ;
	   * }
	   * #Lake depth
	   * 'Lake depth' = {
	   * discipline = 192 ;
	   * parameterCategory = 228 ;
	   * parameterNumber = 7 ;
	   * }
	   * #Convective available potential energy
	   * 'cape' = {
	   * discipline = 0 ;
	   * parameterCategory = 7 ;
	   * parameterNumber = 6 ;
	   * typeOfFirstFixedSurface = 1 ;
	   * typeOfSecondFixedSurface = 8 ;
	   * }
	   * #Minimum temperature at 2 metres since previous post-processing
	   * 'K' = {
	   * discipline = 0 ;
	   * parameterCategory = 0 ;
	   * parameterNumber = 0 ;
	   * scaledValueOfFirstFixedSurface = 15 ;
	   * scaleFactorOfFirstFixedSurface = 1 ;
	   * typeOfStatisticalProcessing = 3 ;
	   * typeOfFirstFixedSurface = 103 ;
	   * is_uerra = 1 ;
	   * }
	   */

// JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
// ORIGINAL LINE: private void parseLocalConcept(NetcdfSharp.com.google.common.collect.ImmutableListMultimap<LocalConceptPart, LocalConceptPart>.Builder localConceptParts, String path, Type conceptType) throws NetcdfSharp.java.io.IOException
	  private void parseLocalConcept(ImmutableListMultimap<LocalConceptPart, LocalConceptPart>.Builder localConceptParts, string path, Type conceptType)
	  {
		using (Stream @is = NetcdfSharp.ucar.nc2.grib.GribResourceReader.getInputStreamOrNull(path))
		{
		  if (@is == null)
		  {
			return; // file not found is ok
		  }
		  using (StreamReader br = new StreamReader(@is, Encoding.UTF8))
		  {
			bool header = true;

			LocalConceptPart current = null;
			while (true)
			{
			  string line = br.ReadLine();
			  if (string.ReferenceEquals(line, null))
			  {
				break; // done with the file
			  }
			  if ((line.Length == 0))
			  {
				continue;
			  }
			  // skip header lines
			  if (header && (line.StartsWith("# Auto", StringComparison.Ordinal) || line.StartsWith("#Provide", StringComparison.Ordinal)))
			  {
				continue;
			  }
			  header = false;

			  // edzw has this extra line
			  if (line.StartsWith("#paramId", StringComparison.Ordinal))
			  {
				continue;
			  }

			  if (line.StartsWith("#", StringComparison.Ordinal))
			  {
				if (current != null)
				{
				  localConceptParts.put(current, current);
				}
				string paramName = line.Substring(1);
				line = br.ReadLine();
				current = new LocalConceptPart(this, paramName, clean(line), conceptType);
				continue;
			  }

			  if (line.Contains("="))
			  {
				IEnumerator<string> tokens = Splitter.on('=').trimResults().omitEmptyStrings().split(line).GetEnumerator();
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				tokens.MoveNext();
				string name = tokens.Current;
// JAVA TO C# CONVERTER TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				tokens.MoveNext();
				string valueS = clean(tokens.Current);
				int value;
				try
				{
				  value = int.Parse(valueS);
				  if (current != null)
				  {
					current.addAttribute(name, value);
				  }
				}
				catch (Exception)
				{
				  logger.warn("Table {}/{} line {}", tableId, conceptType, line);
				}
			  }
			}
			if (current != null)
			{
			  localConceptParts.put(current, current);
			}
		  }
		}
	  }

	  private string clean(string @in)
	  {
		StringBuilder sb = new StringBuilder(@in);
		StringUtil2.removeAll(sb, ";={}'");
		return sb.ToString().Trim();
	  }

	  private class AttributeBag
	  {
		internal readonly IDictionary<string, int> atts = new SortedDictionary<string, int>();
		internal int hashCode;

		public override bool Equals(object o)
		{
		  if (this == o)
		  {
			return true;
		  }
		  if (o == null || this.GetType() != o.GetType())
		  {
			return false;
		  }
		  AttributeBag that = (AttributeBag) o;
		  return atts.Equals(that.atts);
		}

		public override int GetHashCode()
		{
		  // must roll our own hashcode
		  if (hashCode == 0)
		  {
			int result = 17;
			foreach (KeyValuePair<string, int> entry in atts.SetOfKeyValuePairs())
			{
			  result = 37 * result + entry.Key.GetHashCode();
			  result = 37 * result + entry.Value.GetHashCode();
			}
			hashCode = result;
		  }
		  return hashCode;
		}

		public virtual void show(Formatter f)
		{
		  if (atts.Count == 0)
		  {
			return;
		  }
		  f.format("     ");
		  foreach (KeyValuePair<string, int> entry in atts.SetOfKeyValuePairs())
		  {
			f.format("%s(%3d) ", entry.Key, entry.Value);
		  }
		  f.format("%n");
		}
	  }

	  private class LocalConcept : IComparable<LocalConcept>
	  {
		  private readonly EccodesLocalConcepts outerInstance;

		internal readonly string paramName;
		internal readonly LocalConceptPart org;

		internal int discipline;
		internal int category;
		internal int number;
		internal string paramId;
		internal string name;
		internal string shortName;
		internal string units;
		internal string cfName;
		internal string cfVarName;

		internal readonly AttributeBag bag = new AttributeBag();

		internal LocalConcept(EccodesLocalConcepts outerInstance, LocalConceptPart part)
		{
			this.outerInstance = outerInstance;
		  this.paramName = part.paramName;
		  this.org = part;
		  extractValue(part);
		  extractAtts(part);
		}

		internal virtual string Key
		{
			get
			{
			  return paramName + ":" + Grib2Tables.makeParamCode(discipline, category, number);
			}
		}

		internal virtual void merge(LocalConceptPart part)
		{
		  Debug.Assert((this.paramName.Equals(part.paramName)));
		  Debug.Assert((this.org.atts.Equals(part.atts)));
		  extractValue(part);
		}

		internal virtual void extractAtts(LocalConceptPart part)
		{
		  foreach (KeyValuePair<string, int> entry in part.atts.atts.SetOfKeyValuePairs())
		  {
			string name = entry.Key;
			int? value = entry.Value;
			switch (name)
			{
			  case DISCIPLINE:
				discipline = value.Value;
				break;
			  case CATEGORY:
				category = value.Value;
				break;
			  case NUMBER:
				number = value.Value;
				break;
			  default:
				bag.atts[name] = value.Value;
				break;
			}
		  }
		}

		internal virtual void extractValue(LocalConceptPart part)
		{
		  switch (part.type)
		  {
			case NetcdfSharp.ucar.nc2.grib.grib2.table.EccodesLocalConcepts.Type.name:
			  this.name = part.value;
			  break;
			case NetcdfSharp.ucar.nc2.grib.grib2.table.EccodesLocalConcepts.Type.shortName:
			  this.shortName = part.value;
			  break;
			case NetcdfSharp.ucar.nc2.grib.grib2.table.EccodesLocalConcepts.Type.paramId:
			  this.paramId = part.value;
			  break;
			case NetcdfSharp.ucar.nc2.grib.grib2.table.EccodesLocalConcepts.Type.units:
			  this.units = part.value;
			  break;
			case NetcdfSharp.ucar.nc2.grib.grib2.table.EccodesLocalConcepts.Type.cfName:
			  this.cfName = part.value;
			  break;
			case NetcdfSharp.ucar.nc2.grib.grib2.table.EccodesLocalConcepts.Type.cfVarName:
			  this.cfVarName = part.value;
			  break;
		  }
		}

		internal virtual string Code
		{
			get
			{
			  return Grib2Tables.makeParamCode(discipline, category, number);
			}
		}

		internal virtual string Name
		{
			get
			{
			  return !string.ReferenceEquals(paramName, null) ? paramName : cfName;
			}
		}

		internal virtual string ShortName
		{
			get
			{
			  return !string.ReferenceEquals(shortName, null) ? shortName : cfVarName;
			}
		}

		internal virtual string Units
		{
			get
			{
			  return !string.ReferenceEquals(units, null) ? units : "";
			}
		}

		public virtual int CompareTo(LocalConcept o)
		{
		  int c = discipline - o.discipline;
		  if (c != 0)
		  {
			return c;
		  }
		  c = category - o.category;
		  if (c != 0)
		  {
			return c;
		  }
		  return number - o.number;
		}
	  }

	  private enum Type
	  {
		name,
		shortName,
		paramId,
		units,
		cfName,
		cfVarName
	  }
	  private class LocalConceptPart
	  {
		  private readonly EccodesLocalConcepts outerInstance;

		internal readonly AttributeBag atts = new AttributeBag();
		internal readonly string paramName;
		internal readonly string value;
		internal readonly Type type;

		internal LocalConceptPart(EccodesLocalConcepts outerInstance, string paramName, string value, Type type)
		{
			this.outerInstance = outerInstance;
		  this.paramName = paramName;
		  this.value = value;
		  this.type = type;
		}

		internal virtual void addAttribute(string name, int value)
		{
		  atts.atts[name] = value;
		}

		public override bool Equals(object o)
		{
		  if (this == o)
		  {
			return true;
		  }
		  if (o == null || this.GetType() != o.GetType())
		  {
			return false;
		  }

		  LocalConceptPart that = (LocalConceptPart) o;

		  if (!atts.Equals(that.atts))
		  {
			return false;
		  }
		  return paramName.Equals(that.paramName);
		}

		public override int GetHashCode()
		{
		  int result = atts.GetHashCode();
		  result = 31 * result + paramName.GetHashCode();
		  return result;
		}
	  }

	  private const string FORMAT = "%-10s: %-70s: %-10s - %-8s - %-10s - %-20s - %s%n";

	  internal virtual void showDetails(Formatter f)
	  {
// JAVA TO C# CONVERTER TASK: Most Java stream collectors are not converted by Java to C# Converter:
		ImmutableList<LocalConcept> sorted = ImmutableList<LocalConcept>.copyOf(localConcepts.values().OrderBy(c => c));
		ISet<string> attNames = new SortedSet<string>();

		f.format(FORMAT, "code", "name", "shortName", "paramId", "units", "cfName", "cfVarName");
		f.format("%s%n", StringUtil2.padRight("-", 120, "-"));
		foreach (LocalConcept lc in sorted)
		{
		  f.format(FORMAT, lc.Code, lc.paramName, lc.shortName, lc.paramId, lc.units, lc.cfName, lc.cfVarName);
		  lc.bag.show(f);
		  attNames.UnionWith(lc.bag.atts.Keys);
		}
		f.format("%s%n", StringUtil2.padRight("-", 120, "-"));
		f.format("All attribute names in this table:%n");
		foreach (string attName in attNames)
		{
		  f.format(" %s%n", attName);
		}
	  }

	  internal virtual void showEntryDetails(Formatter f, IList<GribTables.Parameter> @params)
	  {
		IList<LocalConcept> concepts = new List<LocalConcept>();
		ISet<string> attNames = new SortedSet<string>();

		foreach (GribTables.Parameter param in @params)
		{
		  string key = param.Name + ":" + Grib2Tables.makeParamCode(param.Discipline, param.Category, param.Number);
		  ICollection<LocalConcept> match = localConcepts.get(key);
		  foreach (LocalConcept concept in match)
		  {
			if (concept.ShortName.Equals(param.Abbrev))
			{
			  concepts.Add(concept);
			  showLocalConcept(f, concept, attNames);
			}
		  }
		}
		f.format("%n");

		int count = 0;
		f.format("%-30s   ", "");
		foreach (LocalConcept concept in concepts)
		{
		  f.format("  (%2d)   ", count);
		  count++;
		}
		f.format("%n");

		foreach (string attName in attNames)
		{
		  f.format("%-30s ", attName);
		  foreach (LocalConcept concept in concepts)
		  {
			int? value = concept.bag.atts.TryGetValue(attName, out int v) ? v : (int?) null;
			f.format("%8s ", value == null ? "" : value);
		  }
		  f.format("%n");
		}
	  }

	  private void showLocalConcept(Formatter f, LocalConcept lc, ISet<string> attNames)
	  {
		f.format(FORMAT, lc.Code, lc.paramName, lc.shortName, lc.paramId, lc.units, lc.cfName, lc.cfVarName);
		lc.bag.show(f);
		attNames.UnionWith(lc.bag.atts.Keys);
		f.format("%n");
	  }

	}

}
