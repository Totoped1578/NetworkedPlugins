﻿using NetworkedPlugins.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalStats
{
    public class AddonConfig : IConfig
    {
        public bool IsEnabled { get; set; } = true;
    }
}
