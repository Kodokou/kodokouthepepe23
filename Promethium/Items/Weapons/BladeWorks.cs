using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Promethium.Items.Weapons
{
    public class BladeWorks : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 10;
            item.name = "Blade Works";
            item.width = 32;
            item.height = 32;
            item.useStyle = 4;
            item.useAnimation = 11;
            item.useTime = 11;
            item.magic = true;
            item.value = Item.buyPrice(0, 2);
            item.mana = 6;
            item.noMelee = true;
            item.toolTip = "Summons various metal blades to fall from the sky"; //please tell me how you write good tooltips ;c
            item.UseSound = Terraria.ID.SoundID.Item1;
        }

        public override bool Autoload(ref string name, ref string texture, System.Collections.Generic.IList<EquipType> equips)
        {
            texture = "Promethium/Items/Weapons/FireGem";
            return true;
        }

        public override bool UseItem(Player player)
        {
            for(int i = 0; i < 3; i++) //if you find a better, cleaner solution than this, please make it, I think this looks ugly af
            {
                int randDiffX = Main.rand.Next(-129, 128);
                float rot;
                Vector2 pos = Main.MouseWorld, diff, vel = Vector2.Zero;
                pos.X += randDiffX;
                pos.Y = player.position.Y - Main.screenHeight / 2f;
                int id = Projectile.NewProjectile(pos, vel, mod.ProjectileType("BladeProj"), item.damage, item.knockBack);
                Projectile current = Main.projectile[id];
                diff = Vector2.Normalize(Main.MouseWorld - pos); 
                vel = diff * 7f;
                rot = (float)Math.Atan2(diff.Y, diff.X) + MathHelper.PiOver4;
                current.rotation = rot;
                current.velocity = vel;
                current.ai[0] = Main.MouseWorld.Y;
            }
            return true;
        }

    }
}

