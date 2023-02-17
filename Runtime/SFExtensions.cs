using System.Collections.Generic;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    public static partial class SFExtensions
    {
        public static bool IsNone(this string value) => string.IsNullOrWhiteSpace(value);

        public static T SFInstantiate<T>(this T prefab) where T : Object, ISFInjectable
        {
            if (prefab == null) return null;

            var obj = Object.Instantiate(prefab);
            SFContextRoot.Container.Inject(obj);
            return obj;
        }

        public static T SFInstantiate<T>(this T prefab, Transform parent) where T : Object, ISFInjectable
        {
            if (prefab == null) return null;
            var obj = Object.Instantiate(prefab, parent);
            SFContextRoot.Container.Inject(obj);
            return obj;
        }

        public static T SFInstantiate<T>(this T prefab, Vector3 position) where T : Object, ISFInjectable
        {
            if (prefab == null) return null;
            var obj = Object.Instantiate(prefab, position, Quaternion.identity);
            SFContextRoot.Container.Inject(obj);
            return obj;
        }

        public static T SFInstantiate<T>(this T prefab, Vector3 position, Quaternion rotation)
            where T : Object, ISFInjectable
        {
            if (prefab == null) return null;
            var obj = Object.Instantiate(prefab, position, rotation);
            SFContextRoot.Container.Inject(obj);
            return obj;
        }

        public static GameObject SFInstantiate(this GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null) return null;
            var obj = Object.Instantiate(prefab, position, rotation);
            SFContextRoot.Container.Inject(obj);
            return obj;
        }

        public static T SFInstantiate<T>(this GameObject prefab, Vector3 position, Quaternion rotation)
            where T : Component
        {
            if (prefab == null) return null;
            var obj = Object.Instantiate(prefab, position, rotation);
            SFContextRoot.Container.Inject(obj);
            return obj.GetComponent<T>();
        }

        public static T SFInstantiate<T>(this T prefab, Vector3 position, Quaternion rotation, Transform parent)
            where T : Object, ISFInjectable
        {
            if (prefab == null) return null;
            var obj = Object.Instantiate(prefab, position, rotation, parent);
            SFContextRoot.Container.Inject(obj);
            return obj;
        }

        public static T SFInstantiate<T>(this T prefab, Transform parent, bool instantiateInWorldSpace)
            where T : Object, ISFInjectable
        {
            if (prefab == null) return null;
            var obj = Object.Instantiate(prefab, parent, instantiateInWorldSpace);
            SFContextRoot.Container.Inject(obj);
            return obj;
        }

        public static GameObject SFInstantiate(this GameObject prefab, Vector3 position, Quaternion rotation,
            Transform parent)
        {
            if (prefab == null) return null;
            var obj = Object.Instantiate(prefab, position, rotation, parent);
            SFContextRoot.Container.Inject(obj);
            return obj;
        }

        public static GameObject SFInstantiate(this GameObject prefab, Transform parent)
        {
            if (prefab == null) return null;
            var obj = Object.Instantiate(prefab, parent);
            SFContextRoot.Container.Inject(obj);
            return obj;
        }
    }
}