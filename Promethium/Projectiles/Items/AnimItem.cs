using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;

namespace Promethium.Projectiles.Items
{
    public abstract class AnimItem : ModProjectile
    {
        public int frames;
        public Texture2D texture;

        public virtual void Initialize() { }
        public virtual void Animate()
        {
            if (frames != 1 && projectile.ai[0]++ % 10 == 0)
                projectile.frame += projectile.ai[0] / 10 % frames * texture.Height;
        }
        public virtual void RealAI() { }

        public override void SetDefaults()
        {
            Initialize();
            projectile.friendly = true;
            texture = mod.GetTexture("Projectiles/" + projectile.name);
            frames = texture.Height / projectile.height;
            Main.projFrames[projectile.type] = frames;
        }
        //removed CanDamage because spears and other melee projectiles
        public override void AI()
        {
            Animate();
            RealAI();
            if (!projectile.aiStyle <= 0)
                base.AI();
        }
    }
}
