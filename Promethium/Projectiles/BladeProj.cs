using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Promethium
{
    public class BladeProj : ModProjectile //please rename this one, I had no inspiration
    {
        public override void SetDefaults()
        {
            projectile.damage = 10;
            projectile.name = "Blade";
            projectile.width = 32; 
            projectile.height = 32;
            projectile.aiStyle = 0;
            projectile.tileCollide = false;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "Promethium/Items/Weapons/FireGem";
            return true;
        }

        public override void AI()
        {
            /*if (!projectile.tileCollide && projectile.position.Y > projectile.ai[0])
            {
                projectile.ai[0] = 0;
                projectile.tileCollide = true;
            }
            else if (projectile.ai[0]++ % 2 == 0)
            {
                projectile.alpha += 5;
                projectile.velocity *= 0.95f;
            }*/

            if (projectile.ai[1] == 0)
            {
                switch (Main.rand.Next(4))
                {
                    case 0: 
                        projectile.ai[1] = ItemID.CopperBroadsword;
                        break;
                    case 1:
                        projectile.ai[1] = ItemID.IronBroadsword;
                        break;
                    case 2:
                        projectile.ai[1] = ItemID.SilverBroadsword;
                        break;
                    case 3:
                        projectile.ai[1] = ItemID.GoldBroadsword;
                        break;
                }

            }
            projectile.netUpdate = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Microsoft.Xna.Framework.Color lightColor)
        {
            Vector2 actualPos = projectile.position - Main.screenPosition;
            spriteBatch.Draw(Main.itemTexture[(int)projectile.ai[1]], actualPos, null, lightColor, projectile.rotation, new Vector2(projectile.width, 0), 1f, SpriteEffects.None, 0);
            return false;
        }
    }
}

