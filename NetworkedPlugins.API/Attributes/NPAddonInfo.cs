using System;

namespace NetworkedPlugins.API.Attributes
{
    public class NPAddonInfo : Attribute
    {
        public string addonID { get; set; }
        public string addonName { get; set; }
        public string addonVersion { get; set; }
        public string addonAuthor { get; set; }
    }
}
