using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Promethium.Projectiles
{
    public class Vortex : ModProjectile
    {
        private const int VACCUM_DIST_SQ = 10000;

        public override void SetDefaults()
        {
			
            projectile.name = "Void Vortex";
            projectile.alpha = 255;
            projectile.width = 20;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.timeLeft = 300;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            Main.projFrames[projectile.type] = 1;
        }

        public override void AI()
        {
            projectile.velocity *= 0.85F;
            if (projectile.alpha > 4) projectile.alpha -= 5;
            for (int i = 0; i < 200; ++i)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(this) && !npc.boss)
                {
                    Vector2 delta = projectile.Center - npc.Center;
                    float dist = delta.LengthSquared();
                    if (dist <= VACCUM_DIST_SQ)
                    {
                        float mult = (1 - dist / VACCUM_DIST_SQ) * 5;
                        npc.Center += Vector2.Normalize(delta) * mult;
                        npc.velocity *= 0.8F;
                    }
                }
            }
        }
    }
}