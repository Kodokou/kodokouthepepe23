using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Promethium.AI.Astar;

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
            Point feetPos = aiu.entity.position.Add(0, aiu.entity.height).ToTileCoordinates();
            bool onGround = false;
            for (int i = (aiu.entity.width + 15) / 16 - 1; i >= 0 && !onGround; --i)
                onGround |= Main.tile[feetPos.X + i, feetPos.Y].IsSolid();
            Vector2 destPos;
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
            bool xZigZag = false, yTakeOff = false;
            Vector2 destDelta = destPos - aiu.entity.position;
            int calcJump = Math.Max((int)(destDelta.Y / 16 - 1), -maxJump);
            if (destDelta.LengthSquared() > 80 * 80)
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
                    //if (plr.HasMinionRestTarget) destPos = aiu.entity.position + Main.rand.NextVector2CircularEdge(16, 8);
                    //else return false;
                }
                else if (path.Count > 0)
                {
                    // TODO: Remove debug
                    foreach (Location l in path)
                        Main.dust[Dust.NewDust(l.Pos.ToEntityPos(aiu.entity), 1, 1, DustID.Fire)].noGravity = true;

                    Location newLoc = path.Last.Value;
                    while (newLoc.Z != 0 && path.Count > 1)
                    {
                        Location nextLoc = path.Last.Previous.Value;
                        if (Collision.CanHitLine(aiu.entity.position, aiu.entity.width, aiu.entity.height, nextLoc.Pos.ToEntityPos(aiu.entity), aiu.entity.width, aiu.entity.height))
                        {
                            path.RemoveLast();
                            newLoc = nextLoc;
                        }
                        else break;
                    }
                    if (path.Count > 1)
                    {
                        Location nextLoc = path.Last.Previous.Value;
                        int newDiff = Math.Sign(newLoc.Pos.ToEntityPos(aiu.entity).X - aiu.entity.position.X);
                        int nextDiff = Math.Sign(nextLoc.Pos.X - newLoc.Pos.X);
                        xZigZag = newDiff != nextDiff;
                        yTakeOff = newLoc.Z == 0 && nextLoc.Z != 0;
                    }
                    if (newLoc.Z == 0) calcJump = (int)Math.Floor((newLoc.Pos.ToEntityPos(aiu.entity).Y - aiu.entity.position.Y) / 16);
                    else
                    {
                        LinkedListNode<Location> it = path.Last;
                        while (it != null)
                        {
                            calcJump = Math.Min(calcJump, (int)Math.Floor((it.Value.Pos.ToEntityPos(aiu.entity).Y - aiu.entity.position.Y) / 16));
                            if (it.Value.Z == 0) break;
                            it = it.Previous;
                        }
                    }
                    int maxDistSq = xZigZag || yTakeOff ? 4 * 4 : 32 * 32;
                    while ((newLoc.Pos.ToEntityPos(aiu.entity) - aiu.entity.position).LengthSquared() < maxDistSq)
                    {
                        path.RemoveLast();
                        if (path.Count > 1)
                        {
                            Location nextLoc = path.Last.Previous.Value;
                            int newDiff = Math.Sign(newLoc.Pos.ToEntityPos(aiu.entity).X - aiu.entity.position.X);
                            int nextDiff = Math.Sign(nextLoc.Pos.X - newLoc.Pos.X);
                            xZigZag = newDiff != nextDiff;
                        }
                        if (path.Count > 0) newLoc = path.Last.Value;
                        else
                        {
                            newLoc.Z = 255;
                            break;
                        }
                    }
                    if (newLoc.Z != 255) destPos = newLoc.Pos.ToEntityPos(aiu.entity);
                }
            }
            else path = null;
            timer = (timer + 1) % 3;
            if (timer == 0) Main.dust[Dust.NewDust(destPos, 1, 1, DustID.Fire)].noGravity = true;

            destDelta = destPos - aiu.entity.position;
            int moveDir = Math.Abs(destDelta.X) > 4 ? Math.Sign(destDelta.X) : 0;

            Vector2 vel = aiu.entity.velocity;
            if (calcJump < -1 && onGround && vel.Y == 0)
            {
                vel.Y = -(float)Math.Sqrt((calcJump * 16 - 8) * -0.84375F);
            }

            float xVelMin = 0.7F;
            float xVelMax = 5;
            if (xVelMax < Math.Abs(plr.velocity.X) + Math.Abs(plr.velocity.Y))
            {
                xVelMax = Math.Abs(plr.velocity.X) + Math.Abs(plr.velocity.Y);
                xVelMin = 0.9F;
            }
            if (moveDir == -1)
            {
                if (vel.X > -4) vel.X -= xVelMin;
                else vel.X -= 0.1F;
            }
            else if (moveDir == 1)
            {
                if (vel.X < 4) vel.X += xVelMin;
                else vel.X += 0.1F;
            }
            else
            {
                vel.X *= 0.85f;
                if (Math.Abs(vel.X) < xVelMin * 2) vel.X = 0;
            }
            if (xZigZag && vel.X > destDelta.X) vel.X = destDelta.X;
            bool colliding = false;
            if (moveDir != 0)
            {
                int projX = (int)(aiu.entity.position.X + aiu.entity.width / 2) / 16 + moveDir + (int)vel.X;
                int projY = (int)aiu.entity.position.Y / 16;
                for (int i = projY; i < projY + aiu.entity.height / 16 + 1; ++i)
                    if (WorldGen.SolidTile(projX, i)) colliding = true;
            }
            aiu.CollisionStepUp();
            if (vel.Y == 0)
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
                                projX = (int)(aiu.entity.position.X + aiu.entity.width / 2 + vel.X) / 16 + moveDir;
                                projY = (int)(aiu.entity.position.Y + aiu.entity.height / 2) / 16;
                                if (!WorldGen.SolidTile(projX, projY - 1) && !WorldGen.SolidTile(projX, projY - 2))
                                    vel.Y = -5.1f;
                                else if (!WorldGen.SolidTile(projX, projY - 2))
                                    vel.Y = -7.1f;
                                else if (WorldGen.SolidTile(projX, projY - 5))
                                    vel.Y = -11.1f;
                                else if (WorldGen.SolidTile(projX, projY - 4))
                                    vel.Y = -10.1f;
                                else vel.Y = -9.1f;
                            }
                            catch { vel.Y = -9.1f; }
                    }
                }
            if (vel.X > xVelMax) vel.X = xVelMax;
            if (vel.X < -xVelMax) vel.X = -xVelMax;
            if (vel.X < 0) aiu.entity.direction = -1;
            if (vel.X > 0) aiu.entity.direction = 1;
            if (vel.X > xVelMin && moveDir == 1) aiu.entity.direction = 1;
            if (vel.X < -xVelMin && moveDir == -1) aiu.entity.direction = -1;
            aiu.SetRotation(0);
            vel.Y += 0.4F;
            if (vel.Y > 10) vel.Y = 10;
            aiu.entity.velocity = vel;
            return true;
        }
    }
}