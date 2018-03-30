using Terraria;
using Terraria.ModLoader;

namespace Promethium.Buffs
{
    class Stun : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Stunned");
            Description.SetDefault("You cannot move!");
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.velocity *= 0.4F;
            if (!npc.noGravity) ++npc.velocity.Y;
        }

        public override void Update(Player plr, ref int buffIndex)
        {
            plr.velocity *= 0.4F;
            plr.velocity.Y += plr.gravity;
        }
    }
}
