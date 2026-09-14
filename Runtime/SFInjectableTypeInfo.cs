using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        internal SFInjectableTypeInfo(Type type, List<FieldInfo> fieldsTemp, List<PropertyInfo> propertiesTemp, List<MethodInfo> methodsTemp)
        {
            Type = type;
            GetFields(fieldsTemp);
            GetProperties(propertiesTemp);
            GetMethods(methodsTemp);
        }

        public override string ToString()
        {
            return Type.Name;
        }

        private void GetFields(List<FieldInfo> fieldsTemp)
        {
            fieldsTemp.Clear();
            var currentType = Type;
            var objectType = typeof(object);
            while (currentType != objectType)
            {
                var fields = currentType.GetFields(BINDING_FLAGS);
                fieldsTemp.AddRange(fields.Where(f => f.GetCustomAttribute<SFInjectAttribute>(true) != null));
                currentType = currentType.BaseType;
            }
            Fields = fieldsTemp.ToArray();
        }

        private void GetProperties(List<PropertyInfo> propertiesTemp)
        {
            propertiesTemp.Clear();
            var currentType = Type;
            var objectType = typeof(object);
            while (currentType != objectType)
            {
                var properties = currentType.GetProperties(BINDING_FLAGS);
                propertiesTemp.AddRange(properties.Where(p => p.GetCustomAttribute<SFInjectAttribute>(true) != null));
                currentType = currentType.BaseType;
            }
            Properties = propertiesTemp.ToArray();
        }

        private void GetMethods(List<MethodInfo> methodsTemp)
        {
            methodsTemp.Clear();
            var currentType = Type;
            var objectType = typeof(object);
            while (currentType != objectType)
            {
                var methods = currentType.GetMethods(BINDING_FLAGS);
                methodsTemp.AddRange(methods.Where(m => m.GetCustomAttribute<SFInjectAttribute>(true) != null));
                currentType = currentType.BaseType;
            }
            Methods = methodsTemp.ToArray();

            ParametersByMethod = new Dictionary<MethodInfo, ParameterInfo[]>();
            foreach (var methodInfo in Methods)
            {
                ParametersByMethod[methodInfo] = methodInfo.GetParameters();
            }
        }
    }
}
