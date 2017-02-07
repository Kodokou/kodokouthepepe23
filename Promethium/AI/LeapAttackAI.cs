using System;
using Terraria;
using Microsoft.Xna.Framework;

namespace Promethium.AI
{
    class LeapAttackAI : BaseAI
    {
        public int startDist = int.MaxValue, maxSpeed = 0;
        public float accel = 0.3F;

        public override bool CanStart(AIUser aiu)
        {
            short targetID = aiu.GetI16Data();
            if (targetID != -1)
            {
                NPC target = Main.npc[targetID];
                return target.CanBeChasedBy(aiu.entity) && (target.Center - aiu.entity.Center).LengthSquared() < startDist;
            }
            else return false;
        }

        public override bool AI(AIUser aiu)
        {
            short targetID = aiu.GetI16Data();
            if (targetID != -1)
            {
                NPC target = Main.npc[targetID];
                if (target.CanBeChasedBy(aiu.entity) && (target.Center - aiu.entity.Center).LengthSquared() < startDist)
                {
                    Vector2 deltaPos = aiu.entity.Center - target.Center;
                    float speedMult = maxSpeed, velSum = Math.Abs(target.velocity.X) + Math.Abs(target.velocity.Y);
                    if (speedMult < velSum) speedMult = velSum;
                    float plrDist = deltaPos.Length();
                    Vector2 vel = aiu.entity.velocity;
                    if (plrDist >= 60)
                    {
                        deltaPos.Normalize();
                        deltaPos *= speedMult;
                        if (vel.X < deltaPos.X)
                        {
                            vel.X += accel;
                            if (vel.X < 0) vel.X += accel * 3 / 2;
                        }
                        else if (vel.X > deltaPos.X)
                        {
                            vel.X -= accel;
                            if (vel.X > 0) vel.X -= accel * 3 / 2;
                        }
                        if (vel.Y < deltaPos.Y)
                        {
                            vel.Y += accel;
                            if (vel.Y < 0) vel.Y += accel * 3 / 2;
                        }
                        else if (vel.Y > deltaPos.Y)
                        {
                            vel.Y -= accel;
                            if (vel.Y > 0) vel.Y -= accel * 3 / 2;
                        }
                    }
                    aiu.SetRotation(vel.X / 10);
                    aiu.entity.velocity = vel;
                    return true;
                }
            }
            return false;
        }
    }
}
