using System.Reflection;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace HealProjectile
{
    [ApiVersion(2, 1)]
    public class HealProjectileMain : TerrariaPlugin
    {
        public HealProjectileMain(Main game) : base(game)
        {
        }

        public override string Author => "Miyabi";

        public override string Description => "Allow recovery when the projectile is hit.";

        public override string Name => "HealProjectile";

        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public Dictionary<int, int> HealProjectile { get; set; } = new Dictionary<int, int>();

        public const string HealProjectileConfigPath = "HealProjectileConfig.json";

        public override void Initialize()
        {
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
            string path = Path.GetFullPath(Path.Combine(TShock.SavePath, HealProjectileConfigPath));
            if (File.Exists(path))
            {
                using (var sr = new StreamReader(path))
                {
                    var setting = JsonConvert.DeserializeObject<List<HealSetting>>(sr.ReadToEnd());
                    foreach (var healSetting in setting)
                    {
                        HealProjectile.Add(healSetting.ProjectileType, healSetting.HealAmount);
                    }
                }
            }
            else
            {
                var setting = new List<HealSetting>()
                {
                    new HealSetting()
                    {
                        ProjectileType = ProjectileID.PurificationPowder,
                        HealAmount = 30,
                    },
                };
                foreach (var healSetting in setting)
                {
                    HealProjectile.Add(healSetting.ProjectileType, healSetting.HealAmount);
                }
                
                using (var sw = new StreamWriter(path))
                {
                    sw.Write(JsonConvert.SerializeObject(setting, Formatting.Indented));
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile != null && projectile.active)
                {
                    if (HealProjectile.ContainsKey(projectile.type))
                    {
                        Player ownerPlayer = Main.player[projectile.owner];
                        if (ownerPlayer == null || !ownerPlayer.active || ownerPlayer.team == 0)
                        {
                            continue;
                        }

                        Rectangle rectangle = projectile.Damage_GetHitbox();
                        for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
                        {
                            if (projectile.owner == playerIndex)
                            {
                                continue;
                            }

                            Player player = Main.player[playerIndex];
                            if (player.active && !player.dead && !player.ghost && player.team == ownerPlayer.team && rectangle.Intersects(Main.player[playerIndex].Hitbox))
                            {
                                NetMessage.SendData((int)PacketTypes.PlayerHealOther, -1, -1, null, playerIndex, HealProjectile[projectile.type]);
                                NetMessage.TrySendData(29, -1, -1, null, projectile.identity, projectile.owner);
                                projectile.Kill();
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}