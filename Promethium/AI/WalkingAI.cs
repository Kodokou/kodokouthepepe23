using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Promethium.AI
{
    public class WalkingAI : BaseAI
    {
        private readonly byte maxJmp;
        private readonly float grav;
        protected readonly PathFinder pathGen = new PathFinder();
        protected LinkedList<Location> path = null;

        public WalkingAI(byte maxJump, float gravity)
        {
            maxJmp = maxJump;
            grav = gravity;
        }

        public override bool CanStart(Entity en)
        {
            return en is Projectile;
        }

        public override bool AI(Entity en)
        {
            Projectile proj = (Projectile)en;
            Player plr = Main.player[proj.owner];

            Vector2 deltaPlr = plr.Center - proj.Center;
            if (Math.Abs(deltaPlr.Y) > 1280 || deltaPlr.Length() > 1600)
            {
                return false;
                /*
                proj.ai[0] = 1;
                proj.netUpdate = true;
                if (proj.velocity.Y > 0 && deltaPlr.Y < 0) proj.velocity.Y = 0;
                if (proj.velocity.Y < 0 && deltaPlr.Y > 0) proj.velocity.Y = 0;
                */
            }

            Vector2 destPos;
            Point feetPos = proj.position.Add(0, proj.height).ToTileCoordinates();
            bool onGround = false;
            for (int i = (proj.width + 15) / 16 - 1; i >= 0 && !onGround; --i)
                onGround |= Main.tile[feetPos.X + i, feetPos.Y].IsSolid();
            if (plr.HasMinionRestTarget)
            {
                destPos = plr.MinionRestTargetPoint;
                destPos.X -= ((proj.minionPos + 1) % 3) * 30 - 30 + 5 * proj.minionPos / 2F * plr.direction;
            }
            else
            {
                destPos = plr.position;
                destPos.Y += plr.height - proj.height - 1;
                destPos.X -= (20 + plr.width / 2F) * plr.direction - plr.width / 2F;
                destPos.X -= ((proj.minionPos % 6) * 30 + 5 * proj.minionPos / 2F) * plr.direction;
            }
            if ((destPos - proj.position).LengthSquared() > 80 * 80)
            {
                if (++proj.ai[1] > 60 || (onGround && proj.ai[1] > 30))
                {
                    proj.ai[1] = 0;
                    path = pathGen.FindPath(proj, destPos.ToTileCoordinates(), maxJmp, 0);
                }
                if (path == null)
                {
                    if (plr.HasMinionRestTarget) destPos = proj.position + Main.rand.NextVector2CircularEdge(16, 8);
                    else return false;
                }
                else if (path.Count > 0)
                {
                    Location newLoc = path.Last.Value;
                    while (newLoc.Z != 0 && path.Count > 1)
                    {
                        Location nextLoc = path.Last.Previous.Value;
                        if (Collision.CanHitLine(proj.position, proj.width, proj.height, nextLoc.Pos.ToWorldCoordinates(0, 16 - proj.height), proj.width, proj.height))
                        {
                            path.RemoveLast();
                            newLoc = nextLoc;
                        }
                        else break;
                    }
                    while ((newLoc.Pos.ToWorldCoordinates(0, 16 - proj.height) - proj.position).LengthSquared() < 24 * 24)
                    {
                        path.RemoveLast();
                        if (path.Count > 0) newLoc = path.Last.Value;
                        else
                        {
                            newLoc.Z = 255;
                            break;
                        }
                    }
                    if (newLoc.Z != 255) destPos = newLoc.Pos.ToWorldCoordinates(0, 16 - proj.height);
                }
            }
            else path = null;
            if (proj.ai[1] % 2 == 0) Main.dust[Dust.NewDust(destPos, 1, 1, DustID.Fire)].noGravity = true;

            Vector2 destDelta = destPos - proj.position;
            int moveDir = Math.Sign(destDelta.X);
            proj.tileCollide = true;

            if (destDelta.Y < -16 && onGround && proj.velocity.Y == 0)
            {
                float jump = Math.Max(destDelta.Y, maxJmp * -16);
                proj.velocity.Y = -(float)Math.Sqrt((jump - 8) * -0.84375F);
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
                if (proj.velocity.X > -4F)
                    proj.velocity.X = proj.velocity.X - xVelMin;
                else proj.velocity.X = proj.velocity.X - 0.1F;
            }
            else if (moveDir == 1)
            {
                if (proj.velocity.X < 4F)
                    proj.velocity.X = proj.velocity.X + xVelMin;
                else proj.velocity.X = proj.velocity.X + 0.1F;
            }
            else
            {
                proj.velocity.X = proj.velocity.X * 0.9f;
                if (Math.Abs(proj.velocity.X) < xVelMin * 2)
                    proj.velocity.X = 0;
            }
            bool colliding = false;
            if (moveDir != 0)
            {
                int projX = (int)(proj.position.X + proj.width / 2) / 16 + moveDir + (int)proj.velocity.X;
                int projY = (int)proj.position.Y / 16;
                for (int i = projY; i < projY + proj.height / 16 + 1; ++i)
                    if (WorldGen.SolidTile(projX, i)) colliding = true;
            }
            Collision.StepUp(ref proj.position, ref proj.velocity, proj.width, proj.height, ref proj.stepSpeed, ref proj.gfxOffY, 1, false, 0);
            if (proj.velocity.Y == 0)
                if (colliding)
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        int projX = (int)(proj.position.X + i * proj.width / 2) / 16;
                        int projY = (int)(proj.position.Y + proj.height) / 16;
                        Tile t = Main.tile[projX, projY];
                        if (t.IsSolid() && !t.IsTopSolid())
                            try
                            {
                                projX = (int)(proj.position.X + proj.width / 2) / 16;
                                projY = (int)(proj.position.Y + proj.height / 2) / 16;
                                projX += moveDir + (int)proj.velocity.X;
                                if (!WorldGen.SolidTile(projX, projY - 1) && !WorldGen.SolidTile(projX, projY - 2))
                                    proj.velocity.Y = -5.1f;
                                else if (!WorldGen.SolidTile(projX, projY - 2))
                                    proj.velocity.Y = -7.1f;
                                else if (WorldGen.SolidTile(projX, projY - 5))
                                    proj.velocity.Y = -11.1f;
                                else if (WorldGen.SolidTile(projX, projY - 4))
                                    proj.velocity.Y = -10.1f;
                                else proj.velocity.Y = -9.1f;
                            }
                            catch { proj.velocity.Y = -9.1f; }
                    }
                }
            if (proj.velocity.X > xVelMax) proj.velocity.X = xVelMax;
            if (proj.velocity.X < -xVelMax) proj.velocity.X = -xVelMax;
            if (proj.velocity.X < 0) proj.direction = -1;
            if (proj.velocity.X > 0) proj.direction = 1;
            if (proj.velocity.X > xVelMin && moveDir == 1) proj.direction = 1;
            if (proj.velocity.X < -xVelMin && moveDir == -1) proj.direction = -1;
            proj.spriteDirection = proj.direction;
            proj.rotation = 0;
            proj.alpha = 0;
            if (proj.velocity.Y == 0 && Math.Abs(proj.velocity.X) >= 0.5f)
            {
                proj.frameCounter += (int)Math.Abs(proj.velocity.X);
                if (++proj.frameCounter > 10)
                {
                    ++proj.frame;
                    proj.frameCounter = 0;
                }
                if (proj.frame > 2) proj.frame = 0;
            }
            else
            {
                proj.frameCounter = 0;
                proj.frame = 0;
            }
            proj.velocity.Y += grav;
            if (proj.velocity.Y > 10) proj.velocity.Y = 10;
            return true;
        }
    }
}