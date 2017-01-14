using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Promethium.Projectiles.Fireball
{
    class FireballSmall : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.name = "Fireball";
            projectile.width = 16;
            projectile.height = 16;
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
                projectile.rotation = projectile.velocity.ToRotation();
            }
        }
    }
}
