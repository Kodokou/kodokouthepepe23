using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Promethium.Projectiles.Fireball
{
    class FireballMed : FireballSmall
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.width = 20;
            projectile.height = 20;
            projectile.penetrate = 2;
        }

        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);
            if (Main.myPlayer == projectile.owner)
                for (int i = 0; i < 3; ++i)
                    Projectile.NewProjectile(projectile.Center, Vector2.UnitX.RotatedBy(Main.rand.NextFloatDirection()) + projectile.velocity / 2, ProjectileID.BallofFire, projectile.damage / 3, projectile.knockBack, projectile.owner);
        }
    }
}
