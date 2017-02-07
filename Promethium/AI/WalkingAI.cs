using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Promethium.AI
{
    public class WalkingAI : BaseAI
    {
        public byte maxJump = byte.MinValue;
        protected readonly PathFinder pathGen = new PathFinder();
        protected LinkedList<Location> path = null;
        private int timer = 0;

        public override bool CanStart(AIUser aiu)
        {
            return true;
        }

        public override bool AI(AIUser aiu)
        {
            Player plr = aiu.GetPlayer();
            Vector2 destPos;
            Point feetPos = aiu.entity.position.Add(0, aiu.entity.height).ToTileCoordinates();
            bool onGround = false;
            for (int i = (aiu.entity.width + 15) / 16 - 1; i >= 0 && !onGround; --i)
                onGround |= Main.tile[feetPos.X + i, feetPos.Y].IsSolid();
            int minionPos = aiu.GetMinionPos();
            if (plr.HasMinionRestTarget)
            {
                destPos = plr.MinionRestTargetPoint;
                destPos.X -= ((minionPos + 1) % 3) * 30 - 30 + 5 * minionPos / 2F * plr.direction;
            }
            else
            {
                destPos = plr.position;
                destPos.Y += plr.height - aiu.entity.height - 1;
                destPos.X -= (20 + plr.width / 2F) * plr.direction - plr.width / 2F;
                destPos.X -= ((minionPos % 6) * 30 + 5 * minionPos / 2F) * plr.direction;
            }
            if ((destPos - aiu.entity.position).LengthSquared() > 80 * 80)
            {
                float pathTimer = aiu.GetFloatData() + 1;
                if (pathTimer > 60 || (onGround && pathTimer > 30))
                {
                    aiu.SetFloatData(0);
                    path = pathGen.FindPath(aiu.entity, destPos.ToTileCoordinates(), maxJump, 0);
                }
                else aiu.SetFloatData(pathTimer);
                if (path == null)
                {
                    if (plr.HasMinionRestTarget) destPos = aiu.entity.position + Main.rand.NextVector2CircularEdge(16, 8);
                    else return false;
                }
                else if (path.Count > 0)
                {
                    Location newLoc = path.Last.Value;
                    while (newLoc.Z != 0 && path.Count > 1)
                    {
                        Location nextLoc = path.Last.Previous.Value;
                        if (Collision.CanHitLine(aiu.entity.position, aiu.entity.width, aiu.entity.height, nextLoc.Pos.ToWorldCoordinates(0, 16 - aiu.entity.height), aiu.entity.width, aiu.entity.height))
                        {
                            path.RemoveLast();
                            newLoc = nextLoc;
                        }
                        else break;
                    }
                    while ((newLoc.Pos.ToWorldCoordinates(0, 16 - aiu.entity.height) - aiu.entity.position).LengthSquared() < 24 * 24)
                    {
                        path.RemoveLast();
                        if (path.Count > 0) newLoc = path.Last.Value;
                        else
                        {
                            newLoc.Z = 255;
                            break;
                        }
                    }
                    if (newLoc.Z != 255) destPos = newLoc.Pos.ToWorldCoordinates(0, 16 - aiu.entity.height);
                }
            }
            else path = null;
            timer = (timer + 1) % 3;
            if (timer == 0) Main.dust[Dust.NewDust(destPos, 1, 1, DustID.Fire)].noGravity = true;

            Vector2 destDelta = destPos - aiu.entity.position;
            int moveDir = Math.Sign(destDelta.X);
            aiu.SetTileCollide(true);

            if (destDelta.Y < -16 && onGround && aiu.entity.velocity.Y == 0)
            {
                float jump = Math.Max(destDelta.Y, maxJump * -16);
                aiu.entity.velocity.Y = -(float)Math.Sqrt((jump - 8) * -0.84375F);
            }

            float xVelMin = 0.5f;
            float xVelMax = 5;
            if (xVelMax < Math.Abs(plr.velocity.X) + Math.Abs(plr.velocity.Y))
            {
                xVelMax = Math.Abs(plr.velocity.X) + Math.Abs(plr.velocity.Y);
                xVelMin = 0.7f;
            }
            if (moveDir == -1)
            {
                if (aiu.entity.velocity.X > -4F)
                    aiu.entity.velocity.X = aiu.entity.velocity.X - xVelMin;
                else aiu.entity.velocity.X = aiu.entity.velocity.X - 0.1F;
            }
            else if (moveDir == 1)
            {
                if (aiu.entity.velocity.X < 4F)
                    aiu.entity.velocity.X = aiu.entity.velocity.X + xVelMin;
                else aiu.entity.velocity.X = aiu.entity.velocity.X + 0.1F;
            }
            else
            {
                aiu.entity.velocity.X = aiu.entity.velocity.X * 0.9f;
                if (Math.Abs(aiu.entity.velocity.X) < xVelMin * 2)
                    aiu.entity.velocity.X = 0;
            }
            bool colliding = false;
            if (moveDir != 0)
            {
                int projX = (int)(aiu.entity.position.X + aiu.entity.width / 2) / 16 + moveDir + (int)aiu.entity.velocity.X;
                int projY = (int)aiu.entity.position.Y / 16;
                for (int i = projY; i < projY + aiu.entity.height / 16 + 1; ++i)
                    if (WorldGen.SolidTile(projX, i)) colliding = true;
            }
            aiu.CollisionStepUp();
            if (aiu.entity.velocity.Y == 0)
                if (colliding)
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        int projX = (int)(aiu.entity.position.X + i * aiu.entity.width / 2) / 16;
                        int projY = (int)(aiu.entity.position.Y + aiu.entity.height) / 16;
                        Tile t = Main.tile[projX, projY];
                        if (t.IsSolid() && !t.IsTopSolid())
                            try
                            {
                                projX = (int)(aiu.entity.position.X + aiu.entity.width / 2) / 16;
                                projY = (int)(aiu.entity.position.Y + aiu.entity.height / 2) / 16;
                                projX += moveDir + (int)aiu.entity.velocity.X;
                                if (!WorldGen.SolidTile(projX, projY - 1) && !WorldGen.SolidTile(projX, projY - 2))
                                    aiu.entity.velocity.Y = -5.1f;
                                else if (!WorldGen.SolidTile(projX, projY - 2))
                                    aiu.entity.velocity.Y = -7.1f;
                                else if (WorldGen.SolidTile(projX, projY - 5))
                                    aiu.entity.velocity.Y = -11.1f;
                                else if (WorldGen.SolidTile(projX, projY - 4))
                                    aiu.entity.velocity.Y = -10.1f;
                                else aiu.entity.velocity.Y = -9.1f;
                            }
                            catch { aiu.entity.velocity.Y = -9.1f; }
                    }
                }
            if (aiu.entity.velocity.X > xVelMax) aiu.entity.velocity.X = xVelMax;
            if (aiu.entity.velocity.X < -xVelMax) aiu.entity.velocity.X = -xVelMax;
            if (aiu.entity.velocity.X < 0) aiu.entity.direction = -1;
            if (aiu.entity.velocity.X > 0) aiu.entity.direction = 1;
            if (aiu.entity.velocity.X > xVelMin && moveDir == 1) aiu.entity.direction = 1;
            if (aiu.entity.velocity.X < -xVelMin && moveDir == -1) aiu.entity.direction = -1;
            aiu.SetRotation(0);
            aiu.entity.velocity.Y += 0.4F;
            if (aiu.entity.velocity.Y > 10) aiu.entity.velocity.Y = 10;
            return true;
        }
    }
}