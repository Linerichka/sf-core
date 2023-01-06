namespace SFramework.Core.Runtime
{
    public interface ISFDatabase
    {
        public string Title { get; }
        public ISFDatabaseNode[] Nodes { get; }
    }
}