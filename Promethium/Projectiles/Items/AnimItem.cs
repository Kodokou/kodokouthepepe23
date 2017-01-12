using Terraria.ModLoader;
using Terraria;

namespace Promethium.Projectiles.Items
{
    public abstract class AnimItem : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.aiStyle = 20;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ownerHitCheck = true;
            Main.projFrames[projectile.type] = 3;
        }

        public override bool CanDamage() { return false; }
    }
}
