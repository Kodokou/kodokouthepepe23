using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace Promethium.Projectiles
{
    class QuantumBeam : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.name = "Quantum Beam";
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
            if (Main.rand.Next(2) == 0)
                Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, projectile.velocity.X / 2, projectile.velocity.Y / 2, 192)].noGravity = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.velocity.Y -= 3 * knockback / projectile.knockBack;
        }

        public override void Kill(int timeLeft)
        {
            Vector2 v = projectile.velocity / 2;
            for (int i = 0; i < 10; ++i)
                Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, v.X, v.Y, 192)].noGravity = true;
        }
    }
}
