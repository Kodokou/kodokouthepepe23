using Terraria;
using Terraria.ModLoader;

namespace Promethium.Buffs
{
    public class ManaBuckler : ModBuff
    {
        int hitCount;

        public override void SetDefaults()
        {
            DisplayName.SetDefault("Mana Buckler");
            Description.SetDefault("");
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player plr, ref int buffIndex)
        {
            hitCount = plr.GetModPlayer<CCMPlayer>(mod).manaBucklerLeft;
        }

        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            tip = "Absorbs damage, draining mana instead for next " + hitCount + " hit" + (hitCount > 1 ? "s" : "");
        }
    }
}