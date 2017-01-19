using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Promethium.Projectiles
{
    public class BladeProjection : ModProjectile
    {
        int item = 0, iW = 16, iH = 16;
        float length = 40;

        public override void SetDefaults()
        {
            projectile.name = "Blade Projection";
            projectile.width = 24; 
            projectile.height = 24;
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
            if (projectile.localAI[1] == 0)
            {
                projectile.localAI[1] = 1;
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
                projectile.velocity /= 10;
                projectile.netUpdate = true;
                item = (int)projectile.ai[0];
                Item temp = new Item();
                temp.SetDefaults(item);
                iW = temp.width;
                iH = temp.height;
                length = (float)Math.Sqrt(iW * iW + iH * iH);
                if (item == ItemID.IceBlade) projectile.coldDamage = true;
                SpawnEffect();
            }
            else if (projectile.timeLeft == 235 && projectile.localAI[1] == 1)
            {
                projectile.velocity *= 10;
                projectile.localAI[1] = 2;
                projectile.netUpdate = true;
            }
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
            for (int i = 0; i < 30; ++i)
            {
                Vector2 v = Main.rand.NextVector2Circular(length, 5).RotatedBy(projectile.rotation);
                int dust = Dust.NewDust(projectile.position + v, 1, 1, 204, projectile.velocity.X / 1.5F, projectile.velocity.Y / 1.5F, 64);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.ai[0] == ItemID.Starfury)
                Projectile.NewProjectile(projectile.position - projectile.velocity * (260 - timeLeft), projectile.velocity, ProjectileID.Starfury, projectile.damage, projectile.knockBack, projectile.owner);
            projectile.velocity /= 1.5F;
            SpawnEffect();
        }

        public override bool PreDraw(SpriteBatch sb, Color lightColor)
        {
            Vector2 actualPos = projectile.position - Main.screenPosition;
            sb.Draw(Main.itemTexture[item], actualPos, null, lightColor * (255 - projectile.alpha), projectile.rotation + MathHelper.PiOver4, new Vector2(iW / 2F, iH / 2F), 1, SpriteEffects.None, 0);
            return false;
        }
    }
}

