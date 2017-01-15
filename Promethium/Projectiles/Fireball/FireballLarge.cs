using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Promethium.Projectiles.Fireball
{
    class FireballLarge : FireballSmall
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.width = 30;
            projectile.height = 30;
            projectile.penetrate = 1;
        }

        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);
            if (Main.myPlayer == projectile.owner)
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileID.InfernoFriendlyBlast, projectile.damage / 5, projectile.knockBack, projectile.owner);
        }
    }
}
