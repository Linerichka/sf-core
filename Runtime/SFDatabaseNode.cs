using System;
using UnityEngine;

namespace SFramework.Core.Runtime
{
    [Serializable]
    public abstract class SFDatabaseNode : ISFDatabaseNode
    {
        public string Name => _name;
        public abstract ISFDatabaseNode[] Children { get; }


        [SerializeField]
        private string _name;
    }
}