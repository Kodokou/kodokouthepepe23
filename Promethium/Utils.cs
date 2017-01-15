using Terraria;
using Terraria.ID;
using System;
using Microsoft.Xna.Framework;

namespace Promethium
{
    public static class Utils
    {
        public static void NetStrikeNPC(NPC npc, int damage, float knockBack, int hitDirection, bool crit = false)
        {
            npc.StrikeNPC(damage, knockBack, hitDirection, crit, false, false);
            if (Main.netMode != 0) NetMessage.SendData(28, -1, -1, "", npc.whoAmI, damage, knockBack, hitDirection, crit ? 1 : 0);
        }

        public static bool IsSolid(Tile t, bool noTopOnly = false)
        {
            return t != null && t.nactive() && (Main.tileSolid[t.type] || (!noTopOnly && Main.tileSolidTop[t.type] && t.frameY == 0));
        }

        public static void DustCircle(Vector2 center, int angle, int radius)
        {
            Main.dust[Dust.NewDust(center + new Vector2(radius, 0).RotatedBy(Math.PI * angle / 50), 1, 1, DustID.Fire, 0, 0, 128)].noGravity = true;
        }
    }
}