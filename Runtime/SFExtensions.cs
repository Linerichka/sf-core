using System.Collections.Generic;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    public static partial class SFExtensions
    {
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


        public static void FindAllPaths(this ISFDatabaseNode[] nodes, out List<string> paths, int targetLayer = -1)
        {
            var ids = new HashSet<string>();
            paths = new List<string>();

            foreach (var root in nodes)
            {
                var childPaths = GetChildPaths(root, "");

                foreach (var path in childPaths)
                {
                    ids.Add(path);
                }
            }

            foreach (var id in ids)
            {
                var split = id.Split("/");
                var path = string.Empty;
                var childLevel = split.Length;

                if (targetLayer > -1 && targetLayer <= split.Length)
                {
                    childLevel = targetLayer;
                }

                for (var i = 0; i < childLevel; i++)
                {
                    path += split[i];
                    if (i < childLevel - 1)
                    {
                        path += "/";
                    }
                }

                if (string.IsNullOrWhiteSpace(path)) continue;

                paths.Add(path);
            }
        }

        private static List<string> GetChildPaths(ISFDatabaseNode node, string path)
        {
            var paths = new List<string>();

            path += node.Name;

            if (node.Children == null)
            {
                paths.Add(path);
                return paths;
            }

            if (node.Children.Length == 0)
            {
                return null;
            }

            foreach (var child in node.Children)
            {
                var childPaths = GetChildPaths(child, path + "/");
                if (childPaths == null) continue;
                paths.AddRange(childPaths);
            }

            return paths;
        }
    }
}