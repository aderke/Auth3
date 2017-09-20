using System.Configuration;

namespace AuthTestApplication.Managers
{
    public class UsersElement: ConfigurationElementCollection
    {
        public UserElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as UserElement;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public new UserElement this[string responseString]
        {
            get { return (UserElement) BaseGet(responseString); }
            set
            {
                if (BaseGet(responseString) != null)
                {
                    BaseRemoveAt(BaseIndexOf(BaseGet(responseString)));
                }
                BaseAdd(value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new UserElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((UserElement) element).Name;
        }
    }
}