// NetcdfSharp.java.util.ServiceLoader compatible shim - minimal subset used by NetcdfSharp.
// Discovers implementations by reflection over the loaded assemblies.
// Results are cached per service type (lazy, on first use) so the expensive
// assembly scan does not run on every call. Java's ServiceLoader caches the
// same way; the previous version re-scanned on every NetcdfFiles.open().
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Collections.Concurrent;

namespace NetcdfSharp.java.util
{
    /// <summary>
    /// Simplified replacement for NetcdfSharp.java.util.ServiceLoader.
    /// load(Type) returns instances of all non-abstract types assignable to the requested type.
    /// The provider set is resolved once per service type and then cached.
    /// </summary>
    public static class ServiceLoader
    {
        private static readonly ConcurrentDictionary<Type, ArrayList> providerCache = new ConcurrentDictionary<Type, ArrayList>();

        public static IEnumerable load(Type service)
        {
            if (service == null)
            {
                return new ArrayList();
            }
            // Resolve (and cache) the provider set once per service type.
            return providerCache.GetOrAdd(service, Resolve);
        }

        private static ArrayList Resolve(Type service)
        {
            var result = new ArrayList();
            Assembly[] assemblies;
            try
            {
                var loaded = AppDomain.CurrentDomain.GetAssemblies().ToList();
                // Also pull in referenced-but-not-yet-loaded assemblies so that IOSPs in
                // sibling projects (e.g. NetcdfSharp.Bufr) are discoverable by reflection.
                foreach (Assembly asm in loaded.ToArray())
                {
                    try
                    {
                        foreach (AssemblyName dep in asm.GetReferencedAssemblies())
                        {
                            if (loaded.All(a => a.GetName().FullName != dep.FullName))
                            {
                                try
                                {
                                    loaded.Add(Assembly.Load(dep));
                                }
                                catch (Exception)
                                {
                                    // ignore assemblies that fail to load
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // ignore assemblies whose references cannot be enumerated
                    }
                }
                assemblies = loaded.ToArray();
            }
            catch (Exception)
            {
                return result;
            }
            foreach (Assembly asm in assemblies)
            {
                Type[] types;
                try
                {
                    types = asm.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = e.Types.Where(t => t != null).ToArray();
                }
                foreach (Type t in types)
                {
                    if (t.IsAbstract || t.IsInterface || !service.IsAssignableFrom(t))
                    {
                        continue;
                    }
                    try
                    {
                        result.Add(Activator.CreateInstance(t));
                    }
                    catch (Exception)
                    {
                        // ignore types that cannot be instantiated
                    }
                }
            }
            return result;
        }
    }
}
