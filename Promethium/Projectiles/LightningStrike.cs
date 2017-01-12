using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace Promethium.Projectiles
{
    class LightningStrike : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.name = "Lightning Strike";
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.timeLeft = 100;
            projectile.penetrate = 1;
            projectile.extraUpdates = 20;
            projectile.hide = true;
            projectile.magic = true;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                if (Main.raining) projectile.damage = projectile.damage * 3 / 2;
                Vector2 v = Main.player[projectile.owner].Center + projectile.velocity * 7;
                projectile.ai[0] = v.X;
                projectile.ai[1] = v.Y;
                projectile.netUpdate = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, mod.ProjectileType("LightningEffect"), 0, 0, projectile.owner, projectile.ai[0], projectile.ai[1]);
        }
    }
}