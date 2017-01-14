using Terraria;

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
    }
}