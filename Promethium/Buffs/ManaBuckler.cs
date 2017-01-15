using System;
using Terraria;
using Terraria.ModLoader;

namespace Promethium.Buffs
{
    public class ManaBuckler : ModBuff
    {
        int hitCount;
        public override void SetDefaults()
        {
            Main.buffName[Type] = "Mana Buckler";
            Main.buffTip[Type] = "";
        }
        public override void Update(Player plr, ref int buffIndex)
        {
            hitCount = plr.GetModPlayer<CCMPlayer>(mod).manaBucklerLeft;
           /* if (plr.ownedProjectileCounts[mod.ProjectileType("ManaBuckler")] <= 0 && plr.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectile(plr.position.X + (float)(plr.width / 2), plr.position.Y + (float)(plr.height / 2), 0f, 0f, mod.ProjectileType("ManaBuckler"), 0, 0f, plr.whoAmI, 0f, 0f);
            }*/
        }
        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            tip = "Converts mana to life for " + hitCount + " hits";
        }
    }
}

