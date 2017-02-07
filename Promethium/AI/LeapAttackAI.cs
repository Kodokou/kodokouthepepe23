using System;
using Terraria;

namespace Promethium.AI
{
    class LeapAttackAI : BaseAI
    {
        public int startDist = int.MaxValue;

        public override bool CanStart(AIUser aiu)
        {
            short targetID = aiu.GetIntData();
            if (targetID != -1)
            {
                NPC target = Main.npc[targetID];
                return target.CanBeChasedBy(aiu.entity) && (target.Center - aiu.entity.Center).LengthSquared() < startDist;
            }
            else return false;
        }

        public override bool AI(AIUser aiu)
        {
            short targetID = aiu.GetIntData();
            if (targetID != -1)
            {
                NPC target = Main.npc[targetID];
                if (target.CanBeChasedBy(aiu.entity) && (target.Center - aiu.entity.Center).LengthSquared() < startDist)
                {
                    // TODO: Accelerate towards target
                    return true;
                }
            }
            return false;
        }
    }
}
