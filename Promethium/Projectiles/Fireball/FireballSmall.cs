﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Promethium.Projectiles.Fireball
{
    class FireballSmall : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.name = "Fireball";
            projectile.width = 10;
            projectile.height = 10;
            projectile.timeLeft = 128;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.aiStyle = 0;
            projectile.penetrate = 2;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                projectile.rotation = projectile.velocity.ToRotation() + (float)System.Math.PI / 4;
            }
            if (Main.rand.Next(2) == 0)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, ProjectileID.Fireball, projectile.velocity.X / 2, projectile.velocity.Y / 2);
        }
    }
}