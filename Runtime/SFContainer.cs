using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    public sealed class SFContainer : ISFContainer
    {
        private static readonly IDictionary<Type, SFInjectableTypeInfo> _injectableTypes;
        private readonly Dictionary<Type, object> _dependencies = new();
        private readonly Dictionary<Type, List<Type>> _mapping = new();

        static SFContainer()
        {
            _injectableTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && typeof(ISFInjectable).IsAssignableFrom(type))
                .Select(type => new SFInjectableTypeInfo(ref type))
                .ToDictionary(typeInfo => typeInfo.Type, t => t);
        }


        public SFContainer()
        {
            Register<ISFContainer, SFContainer>(this);
        }

        public void Register(object instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            var type = instance.GetType();

            if (_dependencies.ContainsKey(type))
            {
                throw new Exception("Object of this type already exists in the dependency container");
            }

            _dependencies[type] = instance;
        }

        public void Register<TService, TImplementation>() where TImplementation : TService
        {
            if (_dependencies.ContainsKey(typeof(TService)))
            {
                throw new Exception("Object of this type already exists in the dependency container");
            }

            object instance;
            var constructor = typeof(TImplementation).GetConstructors().FirstOrDefault();

            if (constructor != null)
            {
                var parameters = constructor.GetParameters();

                if (parameters.Length > 0)
                {
                    var parameterInstances = new object[parameters.Length];
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (!_dependencies.ContainsKey(parameters[i].ParameterType))
                        {
                            throw new Exception(
                                $"Service of type {parameters[i].ParameterType.FullName} is not registered.");
                        }

                        parameterInstances[i] = _dependencies[parameters[i].ParameterType];
                    }

                    instance = constructor.Invoke(parameterInstances);
                }
                else
                {
                    instance = Activator.CreateInstance(typeof(TImplementation));
                }
            }
            else
            {
                instance = Activator.CreateInstance(typeof(TImplementation));
            }

            Register<TService, TImplementation>(instance);
        }

        public void Register<TService, TImplementation>(object instance) where TImplementation : TService
        {
            if (_dependencies.ContainsKey(typeof(TService)))
            {
                throw new Exception("Object of this type already exists in the dependency container");
            }

            Debug.Log($"[Core] Bind: {typeof(TService).Name} to {typeof(TImplementation).Name}");

            foreach (var subclassType in typeof(TService).GetInterfaces())
            {
                if (!_mapping.ContainsKey(subclassType))
                {
                    _mapping[subclassType] = new List<Type>();
                }

                _mapping[subclassType].Add(typeof(TService));
            }

            _dependencies[typeof(TService)] = instance ?? throw new ArgumentNullException(nameof(instance));
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

            InjectFields(ref targetObject, ref injectableType.Fields);
            InjectProperties(ref targetObject, ref injectableType.Properties);
            InjectMethods(ref targetObject, ref injectableType.Methods, ref injectableType.ParametersByMethod);
        }


        private void InjectFields(ref object targetObject, ref IEnumerable<FieldInfo> injectableFields)
        {
            foreach (var fieldInfo in injectableFields)
            {
                var dependency = Resolve(fieldInfo.FieldType);
                fieldInfo.SetValue(targetObject, dependency);
            }
        }

        private void InjectProperties(ref object targetObject, ref IEnumerable<PropertyInfo> injectableProperties)
        {
            foreach (var propertyInfo in injectableProperties)
            {
                var dependency = Resolve(propertyInfo.PropertyType);
                propertyInfo.SetValue(targetObject, dependency);
            }
        }

        private void InjectMethods(ref object targetObject, ref IEnumerable<MethodInfo> methods,
            ref IDictionary<MethodInfo, ParameterInfo[]> parametersByMethod)
        {
            foreach (var methodInfo in methods)
            {
                if (parametersByMethod.TryGetValue(methodInfo, out var parameters))
                {
                    var dependencies = new object[parameters.Length];

                    for (var i = 0; i < methodInfo.GetParameters().Length; i++)
                    {
                        var parameter = parameters[i];
                        var dependency = Resolve(parameter.ParameterType);
                        dependencies[i] = dependency;
                    }

                    methodInfo.Invoke(targetObject, dependencies);
                }
                else
                {
                    methodInfo.Invoke(targetObject, null);
                }
            }
        }
    }
}