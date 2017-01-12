using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Promethium
{
    public class BladeProjection : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.name = "Blade Projection";
            projectile.width = 36; 
            projectile.height = 16;
            projectile.tileCollide = false;
            projectile.timeLeft = 260;
            projectile.friendly = true;
            projectile.magic = true;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "Promethium/Items/Weapons/BladeWorks";
            return true;
        }

        public override void AI()
        {
            if (projectile.frame == 0)
            {
                projectile.frame = (int)projectile.ai[1];
                projectile.ai[1] = 0;
                projectile.netUpdate = true;
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
                projectile.velocity /= 10;
                SpawnEffect();
            }
            else if (projectile.timeLeft == 235) projectile.velocity *= 10;
            if (projectile.ai[1] == 0 && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.ai[1] = 1;
                projectile.netUpdate = true;
            }
            if (projectile.ai[1] != 0) projectile.tileCollide = true;
            if (projectile.localAI[0] == 0) projectile.localAI[0] = 1;
            projectile.alpha += (int)(25 * projectile.localAI[0]);
            if (projectile.alpha > 200)
            {
                projectile.alpha = 200;
                projectile.localAI[0] = -1;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
                projectile.localAI[0] = 1;
            }
            if (projectile.ai[1] == 1)
            {
                projectile.light = 0.9f;
                if (Main.rand.Next(10) == 0)
                    Dust.NewDust(projectile.position, projectile.height, projectile.height, 204, projectile.velocity.X / 2, projectile.velocity.Y / 2, 128, default(Color), 1.2f);
            }
        }

        private void SpawnEffect()
        {
            for (int i = 0; i < 25; ++i)
            {
                Vector2 v = Main.rand.NextVector2Circular(40, 5).RotatedBy(projectile.rotation);
                int dust = Dust.NewDust(projectile.position + v, 1, 1, 204, projectile.velocity.X / 1.5F, projectile.velocity.Y / 1.5F, 64);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            projectile.velocity /= 1.5F;
            SpawnEffect();
        }

        public override bool PreDraw(SpriteBatch sb, Color lightColor)
        {
            Vector2 actualPos = projectile.position - Main.screenPosition;
            sb.Draw(Main.itemTexture[projectile.frame], actualPos, null, lightColor * (255 - projectile.alpha), projectile.rotation + (float)Math.PI / 4, new Vector2(projectile.width / 2F, projectile.height / 2F), 1, SpriteEffects.None, 0);
            return false;
        }
    }
}

