using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    internal sealed class SFInjectableTypeInfo
    {
        public Type Type { get; }
        public IEnumerable<FieldInfo> InjectableFields { get; }
        public IEnumerable<PropertyInfo> InjectableProperties { get; }
        public IEnumerable<MethodInfo> InjectableMethods { get; }

        private const BindingFlags BINDING_FLAGS =
            BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        public SFInjectableTypeInfo(Type type)
        {
            Type = type;
            InjectableFields = GetInjectableFields(type);
            InjectableProperties = GetInjectableProperties(type);
            InjectableMethods = GetInjectableMethods(type);
        }

        public override string ToString()
        {
            return Type.Name;
        }
        
        private IEnumerable<FieldInfo> GetInjectableFields(Type type)
        {
            var fieldInfos = type.GetFields(BINDING_FLAGS);
            return fieldInfos.Where(f => f.GetCustomAttribute<SFInjectAttribute>(true) != null);
        }

        private IEnumerable<PropertyInfo> GetInjectableProperties(Type type)
        {
            var propertyInfos = type.GetProperties(BINDING_FLAGS);
            return propertyInfos.Where(f => f.GetCustomAttribute<SFInjectAttribute>(true) != null);
        }

        private IEnumerable<MethodInfo> GetInjectableMethods(Type type)
        {
            var methodInfos = type.GetMethods(BINDING_FLAGS);
            return methodInfos.Where(f => f.GetCustomAttribute<SFInjectAttribute>(true) != null);
        }
    }
}