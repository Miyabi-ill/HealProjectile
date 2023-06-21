using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealProjectile
{
    internal class HealSetting
    {
        [JsonProperty]
        public int ProjectileType { get; set; }

        [JsonProperty]
        public int HealAmount { get; set; }
    }
}
