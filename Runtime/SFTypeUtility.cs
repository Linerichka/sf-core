using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SFramework.Core.Runtime
{
    public static class SFTypeUtility
    {
        private static readonly IReadOnlyDictionary<string, Type> _typeCache;

        static SFTypeUtility()
        {
            var typeCache = new Dictionary<string, Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("Assembly-CSharp") || a.FullName.StartsWith("UnityEngine") || a.FullName.StartsWith("SF"))
                .ToArray();

            foreach (var assembly in assemblies)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types;
                }

                if (types == null)
                    continue;

                foreach (var type in types)
                {
                    if (type == null)
                        continue;

                    var typeName = type.Name;
                    if (!string.IsNullOrEmpty(typeName))
                    {
                        typeCache.TryAdd(typeName, type);
                    }
                }
            }

            _typeCache = typeCache;
        }

        public static Type GetTypeByName(string typeName)
        {
            return string.IsNullOrEmpty(typeName) ? null : _typeCache.GetValueOrDefault(typeName);
        }
        
        public static object CreateInstance(Type type, params object[] args)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // Choose correct binding flags: public + non-public constructors
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            return Activator.CreateInstance(
                type,
                flags,
                binder: null,
                args: args,
                culture: null
            );
        }
        
        public static T CreateInstance<T>(Type type, params object[] args) where T : class
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // Choose correct binding flags: public + non-public constructors
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var obj = Activator.CreateInstance(
                type,
                flags,
                binder: null,
                args: args,
                culture: null
            );

            return obj as T;
        }
    }
}