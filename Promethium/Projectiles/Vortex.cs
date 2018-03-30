using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Promethium.Projectiles
{
    public class Vortex : ModProjectile
    {
        private const int VACCUM_DIST_SQ = 15000;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Void Vortex");
        }

        public override void SetDefaults()
        {
            projectile.alpha = 255;
            projectile.width = 54;
            projectile.height = 54;
            projectile.friendly = true;
            projectile.timeLeft = 300;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.minion = true;
        }

        public override bool CanDamage()
        {
            return projectile.ai[0] % 3 == 0;
        }

        public override void AI()
        {
            projectile.velocity *= 0.85F;
            if (projectile.timeLeft > 270) projectile.alpha = 255 * (projectile.timeLeft - 270) / 30;
            else if (projectile.timeLeft < 30) projectile.alpha = 255 * (30 - projectile.timeLeft) / 30;
            else
            {
                if (++projectile.ai[0] % 2 == 0)
                {
                    Vector2 pos = projectile.Center;
                    Utils.DustCircle(pos, projectile.timeLeft % 100, 123);
                    for (int i = 0, j = 0; i < Main.maxNPCs && j < 7; ++i)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(projectile) && Collision.CanHitLine(pos, 1, 1, npc.position, npc.width, npc.height))
                        {
                            Vector2 delta = pos - npc.Center;
                            float dist = delta.LengthSquared();
                            if (dist <= VACCUM_DIST_SQ)
                            {
                                float mult = (1 - dist / VACCUM_DIST_SQ) * 5 * (1 + npc.knockBackResist);
                                if (npc.boss)
                                {
                                    mult /= 1.2F;
                                    npc.velocity *= 0.9F;
                                }
                                else npc.velocity *= 0.85F;
                                npc.Center += Vector2.Normalize(delta) * mult;
                            }
                            ++j;
                        }
                    }
                }
                for (int i = 0; i < 2; ++i)
                {
                    Vector2 v = Main.rand.NextVector2Circular(64, 64);
                    int d = Dust.NewDust(projectile.Center - v, 1, 1, 186, v.X / 8, v.Y / 8, 128, Color.Black, 0.666F);
                    Main.dust[d].noGravity = true;
                }
            }
            projectile.rotation -= 0.18F;
        }
    }
}