using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Promethium.Projectiles
{
    public class Vortex : ModProjectile
    {
        private const int VACCUM_DIST_SQ = 15000;

        public override void SetDefaults()
        {
            projectile.name = "Void Vortex";
            projectile.alpha = 255;
            projectile.width = 20;
            projectile.height = 32;
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
            else if (++projectile.ai[0] % 2 == 0)
                for (int i = 0, j = 0; i < 200 && j < 7; ++i)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(this))
                    {
                        Vector2 delta = projectile.Center - npc.Center;
                        float dist = delta.LengthSquared();
                        if (dist <= VACCUM_DIST_SQ)
                        {
                            float mult = (1 - dist / VACCUM_DIST_SQ) * 5 * (1 + npc.knockBackResist);
                            if (npc.boss)
                            {
                                mult /= 1.25F;
                                npc.velocity *= 0.9F;
                            }
                            else npc.velocity *= 0.85F;
                            npc.Center += Vector2.Normalize(delta) * mult;
                        }
                        ++j;
                    }
                }
        }
    }
}