using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;

namespace Promethium.Projectiles
{
    class LightningStrike : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.name = "Lightning Strike";
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.timeLeft = 80;
            projectile.alpha = 255;
            projectile.scale = 0.01F;
            projectile.penetrate = 1;
            projectile.extraUpdates = 20;
            Main.projFrames[projectile.type] = 1;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                if (Main.raining) projectile.damage = projectile.damage * 3 / 2;
                projectile.ai[0] = projectile.position.X;
                projectile.ai[1] = projectile.position.Y;
                projectile.netUpdate = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, mod.ProjectileType("LightningEffect"), 0, 0, projectile.owner, projectile.ai[0], projectile.ai[1]);
            base.Kill(timeLeft);
        }
    }
}