using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using System;

namespace Promethium.Projectiles.Minions
{
    class Skeleton : ModProjectile
    {
        public float necroDrain;
        protected int attackDist;

        public override void SetDefaults()
        {
            projectile.name = "Skeleton";
            projectile.width = 22;
            projectile.height = 26;
            projectile.netImportant = true;
            projectile.timeLeft = 7200;
            projectile.minion = true;
            Main.projPet[projectile.type] = true;
            Main.projFrames[projectile.type] = 3;
            necroDrain = 0.00001F;
            attackDist = 40;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "Promethium/Projectiles/Minions/Skeleton";
            return true;
        }

        public virtual bool Attack(NPC target, float dist)
        {
            projectile.velocity *= 1.01F;
            return dist > 20;
        }

        int lastPath = 0, stillTimer = 0;
        float jumpPower = 17;
        short maxJumpPower = 17;
        PathFinder test = null;
        System.Collections.Generic.IList<Point> path = null;

        private static readonly bool PATHFINDING_AI_TEST = false;

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                projectile.scale = 1 + Main.rand.NextFloat() * 0.2F;
                SpawnEffect();
                test = new PathFinder();
            }
            Player plr = Main.player[projectile.owner];
            if (!plr.active)
            {
                projectile.active = false;
                return;
            }
            if (!plr.dead) projectile.timeLeft = 2;
            if (!PATHFINDING_AI_TEST)
            {
                OldAI();
                return;
            }
            try
            {
                Vector2 deltaPlr = plr.Center - projectile.Center;
                if (Math.Abs(deltaPlr.Y) > 3600 || deltaPlr.Length() > 6000)
                {
                    projectile.ai[0] = 1;
                    projectile.netUpdate = true;
                    if (projectile.velocity.Y > 0 && deltaPlr.Y < 0) projectile.velocity.Y = 0;
                    if (projectile.velocity.Y < 0 && deltaPlr.Y > 0) projectile.velocity.Y = 0;
                }

                if (projectile.ai[0] == 0)
                {
                    Vector2 destPos;
                    Point feetPos = projectile.position.Add(0, projectile.height).ToTileCoordinates();
                    bool onGround = false;
                    for (int i = (projectile.width + 15) / 16 - 1; i >= 0 && !onGround; --i)
                        onGround |= Main.tile[feetPos.X + i, feetPos.Y].IsSolid();
                    if (plr.HasMinionRestTarget)
                    {
                        destPos = plr.MinionRestTargetPoint;
                        destPos.X -= ((projectile.minionPos + 1) % 3) * 30 - 30 + 5 * projectile.minionPos / 2F * plr.direction;
                    }
                    else
                    {
                        destPos = plr.position;
                        destPos.Y += plr.height - projectile.height - 1;
                        destPos.X -= (20 + plr.width / 2F) * plr.direction - plr.width / 2F;
                        destPos.X -= ((projectile.minionPos % 6) * 30 + 5 * projectile.minionPos / 2F) * plr.direction;
                    }
                    if ((destPos - projectile.position).LengthSquared() > 80 * 80)
                    {
                        if (++lastPath > 60 || (onGround && lastPath > 30))
                        {
                            lastPath = 0;
                            path = test.FindPath(projectile, destPos.ToTileCoordinates(), (short)(maxJumpPower - 2), (short)(maxJumpPower - jumpPower));
                        }
                        if (path == null)
                        {
                            if (plr.HasMinionRestTarget) destPos = plr.position.Add(plr.width / 2F, plr.height - projectile.height);
                            else
                            {
                                //projectile.ai[0] = 1;
                                //projectile.netUpdate = true;
                                //if (projectile.velocity.Y > 0 && deltaPlr.Y < 0) projectile.velocity.Y = 0;
                                //if (projectile.velocity.Y < 0 && deltaPlr.Y > 0) projectile.velocity.Y = 0;
                            }
                        }
                        else if (path.Count > 0)
                        {
                            Vector2 newPos = path[path.Count - 1].ToWorldCoordinates(0, 16 - projectile.height);
                            while ((newPos - projectile.position).LengthSquared() < 24 * 24)
                            {
                                path.RemoveAt(path.Count - 1);
                                if (path.Count > 0) newPos = path[path.Count - 1].ToWorldCoordinates(0, 16 - projectile.height);
                                else
                                {
                                    newPos = destPos;
                                    break;
                                }
                            }
                            destPos = newPos;
                        }
                    }
                    else path = null;
                    if (lastPath % 2 == 0) Main.dust[Dust.NewDust(destPos, 1, 1, DustID.Fire)].noGravity = true;

                    Vector2 destDelta = destPos - projectile.position;
                    int moveDir = Math.Sign(destDelta.X);
                    if (destDelta.LengthSquared() > 48 * 48 && projectile.velocity.LengthSquared() < 9 && onGround)
                    {
                        if (++stillTimer > 10)
                        {
                            stillTimer = 0;
                            projectile.velocity += Main.rand.NextVector2CircularEdge(3, 3);
                        }
                    }
                    else stillTimer = 0;
                    projectile.tileCollide = true;

                    if (projectile.velocity.Y == 0 && onGround) jumpPower = maxJumpPower;
                    if (destDelta.Y < -20 && jumpPower > 0 && Math.Abs(projectile.velocity.Y) < 0.5F)
                    {
                        float jump = Math.Max(destDelta.Y, jumpPower * -16);
                        projectile.velocity.Y = -(float)Math.Sqrt((jump - 8) * -0.84375F);
                        jumpPower += jump / 16;
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
                        if (projectile.velocity.X > -4F)
                            projectile.velocity.X = projectile.velocity.X - xVelMin;
                        else projectile.velocity.X = projectile.velocity.X - 0.1F;
                    }
                    else if (moveDir == 1)
                    {
                        if (projectile.velocity.X < 4F)
                            projectile.velocity.X = projectile.velocity.X + xVelMin;
                        else projectile.velocity.X = projectile.velocity.X + 0.1F;
                    }
                    else
                    {
                        projectile.velocity.X = projectile.velocity.X * 0.9f;
                        if (Math.Abs(projectile.velocity.X) < xVelMin * 2)
                            projectile.velocity.X = 0;
                    }
                    bool colliding = false;
                    if (moveDir != 0)
                    {
                        int projX = (int)(projectile.position.X + projectile.width / 2) / 16 + moveDir + (int)projectile.velocity.X;
                        int projY = (int)projectile.position.Y / 16;
                        for (int i = projY; i < projY + projectile.height / 16 + 1; ++i)
                            if (WorldGen.SolidTile(projX, i)) colliding = true;
                    }
                    Collision.StepUp(ref projectile.position, ref projectile.velocity, projectile.width, projectile.height, ref projectile.stepSpeed, ref projectile.gfxOffY, 1, false, 0);
                    if (projectile.velocity.Y == 0)
                        if (colliding)
                        {
                            for (int i = 0; i < 3; ++i)
                            {
                                int projX = (int)(projectile.position.X + i * projectile.width / 2) / 16;
                                int projY = (int)(projectile.position.Y + projectile.height) / 16;
                                Tile t = Main.tile[projX, projY];
                                if (t.IsSolid() && !t.IsTopSolid())
                                    try
                                    {
                                        projX = (int)(projectile.position.X + projectile.width / 2) / 16;
                                        projY = (int)(projectile.position.Y + projectile.height / 2) / 16;
                                        projX += moveDir + (int)projectile.velocity.X;
                                        if (!WorldGen.SolidTile(projX, projY - 1) && !WorldGen.SolidTile(projX, projY - 2))
                                            projectile.velocity.Y = -5.1f;
                                        else if (!WorldGen.SolidTile(projX, projY - 2))
                                            projectile.velocity.Y = -7.1f;
                                        else if (WorldGen.SolidTile(projX, projY - 5))
                                            projectile.velocity.Y = -11.1f;
                                        else if (WorldGen.SolidTile(projX, projY - 4))
                                            projectile.velocity.Y = -10.1f;
                                        else projectile.velocity.Y = -9.1f;
                                    }
                                    catch { projectile.velocity.Y = -9.1f; }
                            }
                        }
                    if (projectile.velocity.X > xVelMax) projectile.velocity.X = xVelMax;
                    if (projectile.velocity.X < -xVelMax) projectile.velocity.X = -xVelMax;
                    if (projectile.velocity.X < 0) projectile.direction = -1;
                    if (projectile.velocity.X > 0) projectile.direction = 1;
                    if (projectile.velocity.X > xVelMin && moveDir == 1) projectile.direction = 1;
                    if (projectile.velocity.X < -xVelMin && moveDir == -1) projectile.direction = -1;
                    projectile.spriteDirection = projectile.direction;
                    projectile.rotation = 0;
                    projectile.alpha = 0;
                    if (projectile.velocity.Y == 0 && Math.Abs(projectile.velocity.X) >= 0.5f)
                    {
                        projectile.frameCounter += (int)Math.Abs(projectile.velocity.X);
                        if (++projectile.frameCounter > 10)
                        {
                            ++projectile.frame;
                            projectile.frameCounter = 0;
                        }
                        if (projectile.frame > 2) projectile.frame = 0;
                    }
                    else
                    {
                        projectile.frameCounter = 0;
                        projectile.frame = 0;
                    }
                    projectile.velocity.Y += 0.4F;
                    if (projectile.velocity.Y > 10) projectile.velocity.Y = 10;
                }
                else if (projectile.ai[0] == 1)
                {
                    projectile.tileCollide = false;
                    float maxDist = 10;
                    if (maxDist < Math.Abs(plr.velocity.X) + Math.Abs(plr.velocity.Y))
                        maxDist = Math.Abs(plr.velocity.X) + Math.Abs(plr.velocity.Y);
                    float plrDist = deltaPlr.Length();
                    if (plrDist > 2000)
                    {
                        SpawnEffect();
                        projectile.position = plr.Center - new Vector2(projectile.width, projectile.height) / 2;
                        SpawnEffect();
                    }
                    if (plrDist < 200 && plr.velocity.Y == 0 && projectile.position.Y + projectile.height <= plr.position.Y + plr.height && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    {
                        projectile.ai[0] = 0;
                        projectile.netUpdate = true;
                        if (projectile.velocity.Y < -6) projectile.velocity.Y = -6;
                    }
                    if (plrDist >= 60)
                    {
                        deltaPlr.Normalize();
                        deltaPlr *= maxDist;
                        if (projectile.velocity.X < deltaPlr.X)
                        {
                            projectile.velocity.X = projectile.velocity.X + 0.2F;
                            if (projectile.velocity.X < 0)
                                projectile.velocity.X = projectile.velocity.X + 0.3F;
                        }
                        if (projectile.velocity.X > deltaPlr.X)
                        {
                            projectile.velocity.X = projectile.velocity.X - 0.2F;
                            if (projectile.velocity.X > 0)
                                projectile.velocity.X = projectile.velocity.X - 0.3f;
                        }
                        if (projectile.velocity.Y < deltaPlr.Y)
                        {
                            projectile.velocity.Y = projectile.velocity.Y + 0.2F;
                            if (projectile.velocity.Y < 0)
                                projectile.velocity.Y = projectile.velocity.Y + 0.3F;
                        }
                        if (projectile.velocity.Y > deltaPlr.Y)
                        {
                            projectile.velocity.Y = projectile.velocity.Y - 0.2F;
                            if (projectile.velocity.Y > 0)
                                projectile.velocity.Y = projectile.velocity.Y - 0.3F;
                        }
                    }
                    if (projectile.velocity.X != 0) projectile.spriteDirection = Math.Sign(projectile.velocity.X);
                    projectile.frameCounter = 0;
                    projectile.frame = 1;
                    projectile.rotation = projectile.velocity.X / 10;
                    projectile.alpha = 128;
                }
            }
            catch (Exception ex) { ErrorLogger.Log(ex.ToString()); throw; }
        }

        private void OldAI()
        {
            Player plr = Main.player[projectile.owner];
            NPC target = null;
            float targetDist = 333;
            if (projectile.ai[0] == 0)
            {
                NPC minionTarget = projectile.OwnerMinionAttackTargetNPC;
                if (minionTarget != null && minionTarget.CanBeChasedBy(projectile, false))
                {
                    float dist = (minionTarget.Center - projectile.Center).Length();
                    if (dist < targetDist * 2)
                    {
                        target = minionTarget;
                        targetDist = dist;
                    }
                }
                if (target == null)
                {
                    for (int i = 0; i < Main.maxNPCs; ++i)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(projectile, false))
                        {
                            float dist = (npc.Center - projectile.Center).Length();
                            if (dist < targetDist)
                            {
                                target = npc;
                                targetDist = dist;
                            }
                        }
                    }
                }
            }
            if (projectile.ai[0] == 1)
            {
                projectile.tileCollide = false;
                float maxDist = 10;
                if (maxDist < Math.Abs(plr.velocity.X) + Math.Abs(plr.velocity.Y))
                {
                    maxDist = Math.Abs(plr.velocity.X) + Math.Abs(plr.velocity.Y);
                }
                Vector2 deltaPlr = plr.Center - projectile.Center;
                float plrDist = deltaPlr.Length();
                if (plrDist > 2000)
                {
                    projectile.position = plr.Center - new Vector2(projectile.width, projectile.height) / 2;
                }
                if (plrDist < 200 && plr.velocity.Y == 0 && projectile.position.Y + projectile.height <= plr.position.Y + plr.height && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[0] = 0;
                    projectile.netUpdate = true;
                    if (projectile.velocity.Y < -6) projectile.velocity.Y = -6;
                }
                if (plrDist >= 60)
                {
                    deltaPlr.Normalize();
                    deltaPlr *= maxDist;
                    if (projectile.velocity.X < deltaPlr.X)
                    {
                        projectile.velocity.X = projectile.velocity.X + 0.2F;
                        if (projectile.velocity.X < 0)
                        {
                            projectile.velocity.X = projectile.velocity.X + 0.3F;
                        }
                    }
                    if (projectile.velocity.X > deltaPlr.X)
                    {
                        projectile.velocity.X = projectile.velocity.X - 0.2F;
                        if (projectile.velocity.X > 0)
                        {
                            projectile.velocity.X = projectile.velocity.X - 0.3f;
                        }
                    }
                    if (projectile.velocity.Y < deltaPlr.Y)
                    {
                        projectile.velocity.Y = projectile.velocity.Y + 0.2F;
                        if (projectile.velocity.Y < 0)
                        {
                            projectile.velocity.Y = projectile.velocity.Y + 0.3F;
                        }
                    }
                    if (projectile.velocity.Y > deltaPlr.Y)
                    {
                        projectile.velocity.Y = projectile.velocity.Y - 0.2F;
                        if (projectile.velocity.Y > 0)
                        {
                            projectile.velocity.Y = projectile.velocity.Y - 0.3F;
                        }
                    }
                }
                if (projectile.velocity.X != 0)
                {
                    projectile.spriteDirection = Math.Sign(projectile.velocity.X);
                }
                projectile.frameCounter = 0;
                projectile.frame = 1;
                projectile.rotation = projectile.velocity.X / 10;
                projectile.alpha = 128;
            }
            if (projectile.ai[0] == 2)
            {
                projectile.friendly = true;
                projectile.spriteDirection = projectile.direction;
                projectile.rotation = 0;
                projectile.frame = 0;
                projectile.velocity.Y += 0.4f;
                if (projectile.velocity.Y > 10) projectile.velocity.Y = 10;
                if (--projectile.ai[1] <= 0)
                {
                    projectile.ai[1] = 0;
                    projectile.ai[0] = 0;
                    projectile.friendly = false;
                    projectile.netUpdate = true;
                    return;
                }
            }
            Vector2 destPos = plr.Center;
            destPos.X -= (15 + plr.width / 2) * plr.direction;
            destPos.X -= ((projectile.minionPos % 6) * 30 + 5 * projectile.minionPos / 2F) * plr.direction;
            if (target != null)
            {
                float maxDist = 800;
                if (projectile.position.Y > Main.worldSurface * 16) maxDist *= 0.7f;
                Vector2 targetPos = target.Center;
                Collision.CanHit(projectile.position, projectile.width, projectile.height, target.position, target.width, target.height);
                if (targetDist < maxDist)
                {
                    destPos = targetPos;
                    if (targetPos.Y < projectile.Center.Y - 30 && projectile.velocity.Y == 0)
                    {
                        float deltaY = Math.Abs(targetPos.Y - projectile.Center.Y);
                        if (deltaY < 120) projectile.velocity.Y = -10;
                        else if (deltaY < 210) projectile.velocity.Y = -13;
                        else if (deltaY < 270) projectile.velocity.Y = -15;
                        else if (deltaY < 310) projectile.velocity.Y = -17;
                        else if (deltaY < 380) projectile.velocity.Y = -18;
                    }
                }
                if (targetDist < attackDist && !Attack(target, targetDist))
                {
                    projectile.ai[0] = 2;
                    projectile.ai[1] = 15;
                    projectile.netUpdate = true;
                }
            }
            if (projectile.ai[0] == 0)
            {
                if (target == null)
                {
                    if (Main.player[projectile.owner].rocketDelay2 > 0)
                    {
                        projectile.ai[0] = 1;
                        projectile.netUpdate = true;
                    }
                    Vector2 deltaPos = plr.Center - projectile.Center;
                    if (deltaPos.Length() > 2000)
                    {
                        projectile.position = plr.Center - new Vector2(projectile.width / 2F, projectile.height / 2F);
                    }
                    else if (deltaPos.Length() > 500 || Math.Abs(deltaPos.Y) > 300)
                    {
                        projectile.ai[0] = 1;
                        projectile.netUpdate = true;
                        if (projectile.velocity.Y > 0 && deltaPos.Y < 0) projectile.velocity.Y = 0;
                        if (projectile.velocity.Y < 0 && deltaPos.Y > 0) projectile.velocity.Y = 0;
                    }
                }
                projectile.tileCollide = true;
                float xVelLimit = 0.5f;
                float xVelClamp = 4;
                if (xVelClamp < Math.Abs(plr.velocity.X) + Math.Abs(plr.velocity.Y))
                {
                    xVelClamp = Math.Abs(plr.velocity.X) + Math.Abs(plr.velocity.Y);
                    xVelLimit = 0.7f;
                }
                int direction = 0;
                bool colliding = false;
                float deltaX = destPos.X - projectile.Center.X;
                if (Math.Abs(deltaX) > 5)
                {
                    if (deltaX < 0)
                    {
                        direction = -1;
                        if (projectile.velocity.X > -4)
                        {
                            projectile.velocity.X = projectile.velocity.X - xVelLimit;
                        }
                        else projectile.velocity.X = projectile.velocity.X - 0.1F;
                    }
                    else
                    {
                        direction = 1;
                        if (projectile.velocity.X < 4)
                        {
                            projectile.velocity.X = projectile.velocity.X + xVelLimit;
                        }
                        else projectile.velocity.X = projectile.velocity.X + 0.1F;
                    }
                }
                else
                {
                    projectile.velocity.X = projectile.velocity.X * 0.9f;
                    if (Math.Abs(projectile.velocity.X) < xVelLimit * 2)
                    {
                        projectile.velocity.X = 0;
                    }
                }
                if (direction != 0)
                {
                    int projX = (int)(projectile.position.X + projectile.width / 2) / 16 + direction + (int)projectile.velocity.X;
                    int projY = (int)projectile.position.Y / 16;
                    for (int i = projY; i < projY + projectile.height / 16 + 1; ++i)
                        if (WorldGen.SolidTile(projX, i)) colliding = true;
                }
                Collision.StepUp(ref projectile.position, ref projectile.velocity, projectile.width, projectile.height, ref projectile.stepSpeed, ref projectile.gfxOffY, 1, false, 0);
                if (projectile.velocity.Y == 0 && colliding)
                {
                    for (int i = 0; i < 3; ++i)
                    {
                        int projX = (int)(projectile.position.X + i * projectile.width / 2) / 16;
                        int projY = (int)(projectile.position.Y + projectile.height) / 16;
                        if (WorldGen.SolidTile(projX, projY) || Main.tile[projX, projY].halfBrick() || Main.tile[projX, projY].slope() > 0 || (TileID.Sets.Platforms[(int)Main.tile[projX, projY].type] && Main.tile[projX, projY].active() && !Main.tile[projX, projY].inActive()))
                        {
                            try
                            {
                                projX = (int)(projectile.position.X + projectile.width / 2) / 16;
                                projY = (int)(projectile.position.Y + projectile.height / 2) / 16;
                                projX += direction + (int)projectile.velocity.X;
                                if (!WorldGen.SolidTile(projX, projY - 1) && !WorldGen.SolidTile(projX, projY - 2))
                                {
                                    projectile.velocity.Y = -5.1f;
                                }
                                else if (!WorldGen.SolidTile(projX, projY - 2))
                                {
                                    projectile.velocity.Y = -7.1f;
                                }
                                else if (WorldGen.SolidTile(projX, projY - 5))
                                {
                                    projectile.velocity.Y = -11.1f;
                                }
                                else if (WorldGen.SolidTile(projX, projY - 4))
                                {
                                    projectile.velocity.Y = -10.1f;
                                }
                                else projectile.velocity.Y = -9.1f;
                            }
                            catch { projectile.velocity.Y = -9.1f; }
                        }
                    }
                }
                if (projectile.velocity.X > xVelClamp) projectile.velocity.X = xVelClamp;
                if (projectile.velocity.X < -xVelClamp) projectile.velocity.X = -xVelClamp;
                if (projectile.velocity.X < 0) projectile.direction = -1;
                if (projectile.velocity.X > 0) projectile.direction = 1;
                if (projectile.velocity.X > xVelLimit && direction == 1) projectile.direction = 1;
                if (projectile.velocity.X < -xVelLimit && direction == -1) projectile.direction = -1;
                projectile.spriteDirection = projectile.direction;
                projectile.rotation = 0;
                projectile.alpha = 0;
                if (projectile.velocity.Y == 0 && Math.Abs(projectile.velocity.X) >= 0.5f)
                {
                    projectile.frameCounter += (int)Math.Abs(projectile.velocity.X);
                    if (++projectile.frameCounter > 10)
                    {
                        ++projectile.frame;
                        projectile.frameCounter = 0;
                    }
                    if (projectile.frame > 2) projectile.frame = 0;
                }
                else
                {
                    projectile.frameCounter = 0;
                    projectile.frame = 0;
                }
                projectile.velocity.Y += 0.4F;
                if (projectile.velocity.Y > 10) projectile.velocity.Y = 10;
            }
        }

        public override bool MinionContactDamage() { return true; }

        public override void Kill(int timeLeft)
        {
            SpawnEffect();
        }

        protected void SpawnEffect()
        {
            // TODO: Change to some bones effect
            for (int i = 0; i < 40; ++i)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0, 0, 32);
        }
    }
}
