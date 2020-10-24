using System;

namespace CSSFormater.Exceptions
{
    public class ConfigurationConstructionException: Exception
    {
        public ConfigurationConstructionException() { }
        public ConfigurationConstructionException(string message) : base(message) { }
    }
}
