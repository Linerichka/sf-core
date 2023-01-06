namespace SFramework.Core.Runtime
{
    public interface ISFDatabaseNode
    {
        public string Name { get; }
        public ISFDatabaseNode[] Children { get; }
    }
}