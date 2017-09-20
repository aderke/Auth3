using System.Configuration;

namespace AuthTestApplication.Managers
{
    public class UserElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }

        [ConfigurationProperty("hash", IsRequired = true)]
        public string Password
        {
            get
            {
                return this["hash"] as string;
            }
        }

        [ConfigurationProperty("role", IsRequired = true)]
        public string Role
        {
            get
            {
                return this["role"] as string;
            }
        }
    }
}