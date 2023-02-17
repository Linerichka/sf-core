using System;
using System.Collections.Generic;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    public interface ISFContainer
    {
        /// <summary>
        /// Inject dependencies on GameObject. Iterate over all components.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="includeInactive"></param>
        void Inject(GameObject gameObject, bool includeInactive = false);

        /// <summary>
        /// Inject dependencies on targetObject
        /// </summary>
        /// <param name="targetObject">Object to add dependencies to</param>
        /// <exception cref="ArgumentNullException"></exception>
        void Inject(object targetObject);

        T Resolve<T>() where T : class;
        object Resolve(Type type);
        T[] ResolveMany<T>();
        object[] Bindings { get; }
    }
}