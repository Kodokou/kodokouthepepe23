using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace Promethium.Projectiles
{
    class VoidOrb : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.name = "Void Orb";
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = 2;
            projectile.friendly = true;
            projectile.timeLeft = 32;
            projectile.ignoreWater = true;
            Main.projFrames[projectile.type] = 1;
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == projectile.owner)
            {
                int vortexId = mod.ProjectileType("Vortex");
                for (int i = 0; i < 200; ++i)
                {
                    Projectile p = Main.projectile[i];
                    if (p.owner == projectile.owner && p.type == vortexId)
                        p.Kill();
                }
                Projectile.NewProjectile(projectile.Center - Vector2.UnitY * 24, Vector2.Zero, vortexId, projectile.damage, projectile.knockBack, projectile.owner);
            }
        }
    }
}
