using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Promethium.Projectiles.Fireball
{
    class FireballSmall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireball");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.timeLeft = 128;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.aiStyle = 0;
            projectile.penetrate = 3;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                projectile.rotation = projectile.velocity.ToRotation() + (float)System.Math.PI / 4;
            }
            Lighting.AddLight(projectile.position, 0.75F, 0.25F, 0.25F);
            if (Main.rand.Next(2) == 0)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, ProjectileID.Fireball, projectile.velocity.X / 2, projectile.velocity.Y / 2);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item20, projectile.position);
            for (int i = 0; i < projectile.damage; ++i)
                Dust.NewDust(projectile.position, projectile.width, projectile.height, ProjectileID.Fireball, projectile.velocity.X / 2, projectile.velocity.Y / 2);
        }
    }
}
