using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Promethium.Projectiles
{
    public class FireGem : ModProjectile
    {
        private const int MAX_DIST_SQ = 16 * 7 * 16 * 7;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire Gem");
        }

        public override void SetDefaults()
        {
            projectile.scale = 0.9F;
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.timeLeft = 60 * 25;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.minion = true;
        }

        public override bool CanDamage() { return projectile.ai[0] == 0; }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = projectile.damage;
                projectile.damage = 1;
                int count = 0, shortestTimeLeft = projectile.timeLeft, shortestID = -1;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.type == projectile.type && p.owner == projectile.owner)
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
            try
            {
                int tileX = (int)(projectile.position.X / 16) - 1;
                int tileWX = (int)((projectile.position.X + projectile.width) / 16) + 2;
                int tileY = (int)(projectile.position.Y / 16) - 1;
                int tileHY = (int)((projectile.position.Y + projectile.height) / 16) + 2;
                if (tileX < 0) tileX = 0;
                if (tileWX > Main.maxTilesX) tileWX = Main.maxTilesX;
                if (tileY < 0) tileY = 0;
                if (tileHY > Main.maxTilesY) tileHY = Main.maxTilesY;
                for (int tx = tileX; tx < tileWX; ++tx)
                    for (int ty = tileY; ty < tileHY; ++ty)
                        if (Utils.IsSolid(Main.tile[tx, ty]))
                        {
                            Vector2 tilePos = new Vector2(tx * 16, ty * 16);
                            if (projectile.position.X + projectile.width - 4 > tilePos.X && projectile.position.X + 4 < tilePos.X + 16 && projectile.position.Y + projectile.height - 4 > tilePos.Y && projectile.position.Y + 4 < tilePos.Y + 16)
                            {
                                projectile.velocity.X = 0;
                                projectile.velocity.Y = -0.45F;
                                if (projectile.ai[0] == 0)
                                {
                                    projectile.ai[0] = 1;
                                    projectile.netUpdate = true;
                                }
                            }
                        }
            }
            catch { }
            if (++projectile.ai[1] > 10)
            {
                projectile.ai[1] = 10;
                if (projectile.velocity.Y == 0 && projectile.velocity.X != 0)
                {
                    projectile.velocity.X = projectile.velocity.X * 0.97f;
                    if (projectile.velocity.X > -0.01 && projectile.velocity.X < 0.01)
                    {
                        projectile.velocity.X = 0;
                        projectile.netUpdate = true;
                    }
                }
                projectile.velocity.Y += 0.45F;
            }
            projectile.rotation += projectile.velocity.X / 40;
            if (projectile.ai[0] == 1)
            {
                Vector2 pos = projectile.Center;
                if (Main.rand.Next(6) == 0) Dust.NewDust(pos, 4, 4, DustID.Smoke);
                Utils.DustCircle(pos, projectile.timeLeft % 100, 16 * 7);
                for (int i = 0; i < Main.maxNPCs; ++i)
                {
                    NPC n = Main.npc[i];
                    if (n.CanBeChasedBy(projectile))
                    {
                        Vector2 diff = pos - n.Center;
                        if (diff.LengthSquared() <= MAX_DIST_SQ)
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
        }

        public override void Kill(int timeLeft)
        {
            Vector2 pos = projectile.Center;
            if (Main.myPlayer == projectile.owner)
                for (int i = 0; i < Main.maxNPCs; ++i)
                {
                    NPC n = Main.npc[i];
                    if (n.CanBeChasedBy(projectile))
                    {
                        Vector2 diff = pos - n.Center;
                        if (diff.LengthSquared() <= MAX_DIST_SQ)
                            Utils.NetStrikeNPC(n, (int)projectile.localAI[0], projectile.knockBack, -Math.Sign(diff.X));
                    }
                }
            Main.PlaySound(SoundID.Item14, pos);
            for (int i = 50; i > 0; --i)
            {
                Vector2 vel = Main.rand.NextVector2Circular(7, 7);
                Dust.NewDust(pos, 4, 4, DustID.Smoke, vel.X, vel.Y, 0, default(Color), 1.4F);
                if (Main.rand.Next(3) == 0)
                {
                    vel *= 1.2F;
                    Dust.NewDust(pos, 4, 4, DustID.Fire, vel.X, vel.Y, 0, default(Color), 1.6F);
                }
            }
        }
    }
}

