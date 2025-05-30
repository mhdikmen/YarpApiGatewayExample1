namespace BuildingBlocks.Exceptions
{
    public class RequiredConfigurationException : Exception
    {
        public RequiredConfigurationException(string configurationKey) : base($"{configurationKey} is not configured.")
        {
        }
    }
}
