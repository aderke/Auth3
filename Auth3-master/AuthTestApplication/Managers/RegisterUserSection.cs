using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AuthTestApplication.Managers
{
    public class RegisterUserSection: ConfigurationSection
    {
        public static RegisterUserSection GetConfig()
        {
            return (RegisterUserSection) ConfigurationManager.GetSection("RegisterUsers") ?? new RegisterUserSection();
        }

        [ConfigurationProperty("Users")]
        [ConfigurationCollection(typeof(UsersElement), AddItemName = "User")]
        public UsersElement Users
        {
            get
            {
                object o = this["Users"];
                return o as UsersElement;
            }
        }

        public List<UserElement> GetUsers()
        {
            return Users.Cast<UserElement>().ToList();
        }
    }
}