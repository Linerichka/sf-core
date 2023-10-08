using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    internal sealed class SFInjectableTypeInfo
    {
        internal Type Type;
        internal FieldInfo[] Fields;
        internal PropertyInfo[] Properties;
        internal MethodInfo[] Methods;
        internal Dictionary<MethodInfo, ParameterInfo[]> ParametersByMethod;

        private const BindingFlags BINDING_FLAGS =
            BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        internal SFInjectableTypeInfo(ref Type type)
        {
            Type = type;
            GetFields();
            GetProperties();
            GetMethods();
            Debug.Log("Test");
        }

        public override string ToString()
        {
            return Type.Name;
        }

        private void GetFields()
        {
            var fieldInfos = Type.GetFields(BINDING_FLAGS);
            Fields =  fieldInfos.Where(f => f.GetCustomAttribute<SFInjectAttribute>(true) != null).ToArray();
        }

        private void GetProperties()
        {
            var propertyInfos = Type.GetProperties(BINDING_FLAGS);
            Properties =  propertyInfos.Where(f => f.GetCustomAttribute<SFInjectAttribute>(true) != null).ToArray();
        }

        private void GetMethods()
        {
            var methodInfos = Type.GetMethods(BINDING_FLAGS);
            Methods = methodInfos.Where(f => f.GetCustomAttribute<SFInjectAttribute>(true) != null).ToArray();
            ParametersByMethod = new Dictionary<MethodInfo, ParameterInfo[]>();
            foreach (var methodInfo in Methods)
            {
                ParametersByMethod[methodInfo] = methodInfo.GetParameters();
            }
        }
    }
}