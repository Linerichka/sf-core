using UnityEngine;

namespace SFramework.Core.Runtime
{
    public abstract class SFDatabase : ScriptableObject, ISFDatabase
    {
        public abstract string Title { get; }
        public abstract ISFDatabaseNode[] Nodes { get; }
    }
}