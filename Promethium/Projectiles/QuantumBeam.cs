using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Promethium.Projectiles
{
    class QuantumBeam : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Quantum Beam");
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 42;
            projectile.aiStyle = 28;
            projectile.alpha = 255;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.timeLeft = 16;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (projectile.alpha > 30) projectile.alpha -= 30;
            projectile.velocity.Y -= 0.5F;
            projectile.velocity.X += projectile.velocity.X > 0 ? 0.25F : -0.25F;
            if (Main.rand.Next(2) == 0)
                Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, projectile.velocity.X / 2, projectile.velocity.Y / 2, 192)].noGravity = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Utils.CustomKnockback(target, knockback, 0, -2);
        }

        public override void Kill(int timeLeft)
        {
            Vector2 v = projectile.velocity / 2;
            for (int i = 0; i < 10; ++i)
                Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, v.X, v.Y, 192)].noGravity = true;
        }
    }
}