using System;
using Terraria;
using Microsoft.Xna.Framework;

namespace Promethium.AI
{
    class GhostFollowAI : BaseAI
    {
        public int startDist = int.MaxValue;

        public override bool CanStart(AIUser aiu)
        {
            Vector2 deltaPlr = aiu.GetPlayer().Center - aiu.entity.Center;
            return Math.Abs((int)deltaPlr.Y) > startDist * 8 / 10 || deltaPlr.LengthSquared() > startDist * startDist;
        }

        public override bool AI(AIUser aiu)
        {
            Player plr = aiu.GetPlayer();
            Vector2 deltaPlr = aiu.entity.Center - plr.Center;
            aiu.SetTileCollide(false);
            float speedMult = 10, velSum = Math.Abs(plr.velocity.X) + Math.Abs(plr.velocity.Y);
            if (speedMult < velSum) speedMult = velSum;
            float plrDist = deltaPlr.Length();
            Vector2 vel = aiu.entity.velocity;
            if (plrDist >= 60)
            {
                deltaPlr.Normalize();
                deltaPlr *= speedMult;
                if (vel.X < deltaPlr.X)
                {
                    vel.X += 0.2F;
                    if (vel.X < 0) vel.X += 0.3F;
                }
                if (vel.X > deltaPlr.X)
                {
                    vel.X -= 0.2F;
                    if (vel.X > 0) vel.X -= 0.3f;
                }
                if (vel.Y < deltaPlr.Y)
                {
                    vel.Y += 0.2F;
                    if (vel.Y < 0) vel.Y += 0.3F;
                }
                if (vel.Y > deltaPlr.Y)
                {
                    vel.Y -= 0.2F;
                    if (vel.Y > 0) vel.Y -= 0.3F;
                }
            }
            aiu.SetRotation(vel.X / 10);
            aiu.entity.velocity = vel;
            return plrDist < 200 && plr.velocity.Y == 0 && aiu.entity.position.Y + aiu.entity.height <= plr.position.Y + plr.height && !Collision.SolidCollision(aiu.entity.position, aiu.entity.width, aiu.entity.height);
        }
    }
}
