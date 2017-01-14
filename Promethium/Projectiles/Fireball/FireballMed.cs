using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Promethium.Projectiles.Fireball
{
    class FireballMed : FireballSmall
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.width = 20;
            projectile.height = 20;
            projectile.penetrate = 3;
        }
    }
}
