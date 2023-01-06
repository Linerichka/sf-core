namespace SFramework.Core.Runtime
{
    public interface ISFDatabaseGenerator
    {
        void GetGenerationData(out SFGenerationData[] generationData);
    }
}