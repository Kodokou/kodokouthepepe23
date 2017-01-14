using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Promethium.Projectiles.Fireball
{
    class FireballLarge : FireballSmall
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.width = 30;
            projectile.height = 30;
            projectile.penetrate = 4;
        }
    }
}
