using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Promethium.Projectiles
{
    public class HeavyFlail : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.name = "HeavyFlail";
            projectile.width = 26;
            projectile.height = 26;
            projectile.aiStyle = 15;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(projectile.velocity.LengthSquared() * 0.25f) + damage;
        }

        public override bool PreDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModLoader.GetTexture("Promethium/Projectiles/HeavyFlailChain");

            Vector2 position = projectile.Center;
            Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;
            Microsoft.Xna.Framework.Rectangle? sourceRectangle = new Microsoft.Xna.Framework.Rectangle?();
            Vector2 origin = new Vector2((float)texture.Width * 0.5f, (float)texture.Height * 0.5f);
            float num1 = (float)texture.Height;
            Vector2 diff = mountedCenter - position;
            float rotation = (float)Math.Atan2((double)diff.Y, (double)diff.X) - 1.57f;
            bool draw = true;
            if (float.IsNaN(position.X) && float.IsNaN(position.Y))
                draw = false;
            if (float.IsNaN(diff.X) && float.IsNaN(diff.Y))
                draw = false;
            while (draw)
            {
                if ((double)diff.Length() < (double)num1 + 1.0)
                {
                    draw = false;
                }
                else
                {
                    Vector2 step = diff;
                    step.Normalize();
                    position += step * num1;
                    diff = mountedCenter - position;
                    Microsoft.Xna.Framework.Color lighting = Lighting.GetColor((int)position.X / 16, (int)((double)position.Y / 16.0));
                    lighting = projectile.GetAlpha(lighting);
                    Main.spriteBatch.Draw(texture, position - Main.screenPosition, sourceRectangle, lighting, rotation, origin, 1f, SpriteEffects.None, 0.0f);
                }
            }

            return true;
        }
    }

}

