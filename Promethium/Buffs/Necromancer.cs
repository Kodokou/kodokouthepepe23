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
        }

        public override void Update(Player plr, ref int buffIndex)
        {
            souls = plr.GetModPlayer<CCMPlayer>(mod).statNecro;
        }

        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            tip = "Power in possesion: " + souls;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "Promethium/Buffs/ManaBuckler";
            return base.Autoload(ref name, ref texture);
        }
    }
}

