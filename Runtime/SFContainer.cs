using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    public sealed class SFContainer : ISFContainer
    {
        private static readonly IDictionary<Type, SFInjectableTypeInfo> _injectableTypes;
        private readonly Dictionary<Type, object> _dependencies;
        private readonly Dictionary<Type, List<Type>> _mapping;

        static SFContainer()
        {
            _injectableTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && typeof(ISFInjectable).IsAssignableFrom(t))
                .Select(t => new SFInjectableTypeInfo(t))
                .ToDictionary(t => t.Type, t => t);
        }

        public SFContainer()
        {
            _mapping = new Dictionary<Type, List<Type>>();
            _dependencies = new Dictionary<Type, object>();
            Bind<ISFContainer>(this);
        }

        public void Bind(object instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();

            if (_dependencies.ContainsKey(type))
            {
                throw new Exception("Object of this type already exists in the dependency container");
            }

            _dependencies[type] = instance;
        }

        public void Bind<T>(object instance)
        {
            if (_dependencies.ContainsKey(typeof(T)))
            {
                throw new Exception("Object of this type already exists in the dependency container");
            }
            
            Debug.Log($"[Core] Bind: {typeof(T).Name}");
            
            foreach (var subclassType in typeof(T).GetInterfaces())
            {
                if (!_mapping.ContainsKey(subclassType))
                {
                    _mapping[subclassType] = new List<Type>();
                }

                _mapping[subclassType].Add(typeof(T));
            }

            _dependencies[typeof(T)] = instance ?? throw new ArgumentNullException(nameof(instance));
        }


        public T Resolve<T>() where T : class
        {
            return Resolve(typeof(T)) as T;
        }

        public T[] ResolveMany<T>()
        {
            if (!_mapping.ContainsKey(typeof(T))) return Array.Empty<T>();

            var result = new T[_mapping[typeof(T)].Count];

            for (int i = 0; i < _mapping[typeof(T)].Count; i++)
            {
                var type = _mapping[typeof(T)][i];
                result[i] = (T)Resolve(type);
            }

            return result;
        }

        public object Resolve(Type type)
        {
            if (_dependencies.ContainsKey(type))
            {
                return _dependencies[type];
            }

            return _dependencies.FirstOrDefault(kvp => type.IsAssignableFrom(kvp.Key)).Value;
        }

        public object[] Bindings => _dependencies.Values.ToArray();


        public IEnumerable<T> GetDependencies<T>() where T : class
        {
            var result = new List<T>();

            foreach (var dependency in _dependencies)
            {
                var typedDependency = dependency.Value as T;
                if (typedDependency != null)
                {
                    result.Add(typedDependency);
                }
            }

            return result;
        }

        /// <summary>
        /// Inject all dependencies in container
        /// </summary>
        public void Inject()
        {
            foreach (var dependency in _dependencies.Values)
            {
                Inject(dependency);
            }
        }

        public void Inject(GameObject gameObject, bool includeInactive = false)
        {
            foreach (var injectable in gameObject.GetComponentsInChildren<ISFInjectable>(includeInactive))
            {
                Inject(injectable);
            }
        }

        public void Inject(object targetObject)
        {
            if (targetObject == null) throw new ArgumentNullException(nameof(targetObject));

            if (!_injectableTypes.TryGetValue(targetObject.GetType(), out var injectableType)) return;

            var injectableFields = injectableType.InjectableFields;
            var injectableProperties = injectableType.InjectableProperties;
            var injectableMethods = injectableType.InjectableMethods;

            InjectFields(ref targetObject, ref injectableFields);
            InjectProperties(ref targetObject, ref injectableProperties);
            InjectMethods(ref targetObject, ref injectableMethods);
        }

        private void InjectFields(ref object targetObject, ref IEnumerable<FieldInfo> injectableFields)
        {
            foreach (var field in injectableFields)
            {
                var dependency = Resolve(field.FieldType);
                field.SetValue(targetObject, dependency);
            }
        }

        private void InjectProperties(ref object targetObject, ref IEnumerable<PropertyInfo> injectableProperties)
        {
            foreach (var property in injectableProperties)
            {
                var dependency = Resolve(property.PropertyType);
                property.SetValue(targetObject, dependency);
            }
        }

        private void InjectMethods(ref object targetObject, ref IEnumerable<MethodInfo> injectableMethods)
        {
            foreach (var method in injectableMethods)
            {
                var parameters = method.GetParameters();

                var dependencies = new object[parameters.Length];

                for (var i = 0; i < method.GetParameters().Length; i++)
                {
                    var parameter = parameters[i];
                    var dependency = Resolve(parameter.ParameterType);
                    dependencies[i] = dependency;
                }

                method.Invoke(targetObject, dependencies);
            }
        }
    }
}