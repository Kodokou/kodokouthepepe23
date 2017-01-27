using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Promethium.Projectiles.Minions
{
    class SkeletonWarrior : Skeleton
    {
        private float oldRot = 0;
        public Texture2D item = null;

        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.name = "Skeleton Warrior";
            projectile.penetrate = -1;
            item = Main.itemTexture[ItemID.BoneSword];
            necroDrain = 0.00025F;
            attackDist = 20;
        }

        public override bool Attack(NPC target, float dist)
        {
            return false;
        }

        public override void PostDraw(SpriteBatch sb, Color lightColor)
        {
            int offset = ((projectile.frame + 1) % 3) * 2;
            Vector2 pos = projectile.Center - Main.screenPosition + new Vector2(projectile.direction * projectile.width / 2F, offset - projectile.height / 4F);
            oldRot = oldRot * 0.95F + (projectile.rotation + MathHelper.PiOver4 * (offset / 3 - 1) / 2) * projectile.direction / 20F;
            sb.Draw(item, pos, null, lightColor * ((255 - projectile.alpha) / 255F), oldRot, item.Size() / 2, 0.666F, projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }
    }
}
