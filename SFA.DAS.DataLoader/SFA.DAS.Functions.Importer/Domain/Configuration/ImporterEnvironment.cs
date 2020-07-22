namespace SFA.DAS.Functions.Importer.Domain.Configuration
{
    public class ImporterEnvironment
    {
        public virtual string EnvironmentName { get; }

        public ImporterEnvironment(string environmentName)
        {
            EnvironmentName = environmentName;
        }
    }
}