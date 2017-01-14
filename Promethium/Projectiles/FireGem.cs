using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Promethium.Projectiles
{
    public class FireGem : ModProjectile
    {
        private const int MAX_DIST_SQ = 16 * 7 * 16 * 7;

        public override void SetDefaults()
        {
            projectile.name = "Fire Gem";
            projectile.scale = 0.9F;
            projectile.width = 18;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.timeLeft = 60 * 25;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.minion = true;
            Main.projFrames[projectile.type] = 1;
        }

        public override bool CanDamage() { return projectile.ai[0] == 0; }

        public override void AI()
        {
            if (projectile.ai[1] == 0)
            {
                projectile.ai[1] = projectile.damage;
                projectile.damage = 1;
                int count = 0, shortestTimeLeft = projectile.timeLeft, shortestID = -1;
                for (int i = 0; i < 200; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.type == mod.ProjectileType("FireGem") && p.owner == projectile.owner)
                    {
                        if (p.timeLeft < shortestTimeLeft)
                        {
                            shortestTimeLeft = p.timeLeft;
                            shortestID = i;
                        }
                        ++count;
                    }
                }
                if (count > 3) Main.projectile[shortestID].Kill();
            }
            Vector2 tilePos = projectile.Center / 16;
            tilePos = new Vector2(tilePos.X, tilePos.Y + 0.5F);
            if (tilePos.X >= 0 && tilePos.X < Main.maxTilesX && tilePos.Y >= 0 && tilePos.Y < Main.maxTilesY)
            {
                if (Utils.IsSolid(Main.tile[(int)tilePos.X, (int)tilePos.Y]))
                {
                    projectile.velocity = Vector2.Zero;
                    if (projectile.ai[0] == 0)
                    {
                        projectile.ai[0] = 1;
                        projectile.netUpdate = true;
                    }
                }
                else projectile.velocity.Y += 0.6F;
            }
            if (projectile.ai[0] == 1)
            {
                if (Main.rand.Next(6) == 0) Dust.NewDust(projectile.Center, 4, 4, DustID.Smoke);
                for (int i = 0; i < 200; ++i)
                {
                    NPC n = Main.npc[i];
                    if (n.CanBeChasedBy(projectile))
                    {
                        Vector2 diff = projectile.Center - n.Center;
                        float distSq = diff.LengthSquared();
                        if (distSq <= MAX_DIST_SQ)
                        {
                            projectile.netUpdate = true;
                            projectile.ai[0] = 2;
                        }
                    }
                }
            }
            else if (projectile.ai[0] > 1)
            {
                if (Main.rand.Next(4) == 0) Dust.NewDust(projectile.Center, 4, 4, DustID.Fire);
                if (++projectile.ai[0] > 45) projectile.Kill();
            }
            projectile.rotation += projectile.velocity.X * 0.02F;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == projectile.owner)
                for (int i = 0; i < 200; ++i)
                {
                    NPC n = Main.npc[i];
                    if (projectile.CanHit(n))
                    {
                        Vector2 diff = projectile.Center - n.Center;
                        float distSq = diff.LengthSquared();
                        if (distSq <= MAX_DIST_SQ)
                            Utils.NetStrikeNPC(n, (int)projectile.ai[1], projectile.knockBack, -Math.Sign(diff.X));
                    }
                }
            Main.PlaySound(SoundID.Item62, projectile.Center);
            for (int i = Main.rand.Next(27, 33); i > 0; --i)
            {
                Vector2 vel = Main.rand.NextVector2Circular(7, 7);
                Dust.NewDust(projectile.Center, 4, 4, DustID.Smoke, vel.X, vel.Y);
                if (Main.rand.Next(4) == 0)
                {
                    vel *= 1.2F;
                    Dust.NewDust(projectile.Center, 4, 4, DustID.Fire, vel.X, vel.Y);
                }
            }
        }
    }
}

