﻿using NetworkedPlugins.API.Attributes;
using NetworkedPlugins.API.Interfaces;
using System.Collections.Generic;

namespace NetworkedPlugins.API
{
    public class NPAddonItem
    {
        public object addon { get; set; }
        public NPAddonInfo info { get; set; }
    }
}
