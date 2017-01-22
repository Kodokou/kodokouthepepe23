using System;
using Terraria;
using Terraria.ModLoader;

namespace Promethium.Buffs
{
    public class Necromancer : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffName[Type] = "Necromancer";
            Main.buffTip[Type] = "";
        }

        public override void Update(Player plr, ref int buffIndex)
        {
            plr.GetModPlayer<CCMPlayer>(mod).statNecro = 15;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "Promethium/Buffs/ManaBuckler";
            return base.Autoload(ref name, ref texture);
        }
    }
}

