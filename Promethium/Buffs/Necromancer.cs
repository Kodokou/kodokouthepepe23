using System;
using Terraria;
using Terraria.ModLoader;

namespace Promethium.Buffs
{
    public class Necromancer : ModBuff
    {
        int souls = 0;

        public override void SetDefaults()
        {
            Main.buffName[Type] = "Soul Prison";
            Main.buffTip[Type] = "";
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player plr, ref int buffIndex)
        {
            souls = (int)plr.GetModPlayer<CCMPlayer>(mod).statNecro;
        }

        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            tip = souls + " soul power in possesion";
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "Promethium/Buffs/ManaBuckler";
            return base.Autoload(ref name, ref texture);
        }
    }
}

