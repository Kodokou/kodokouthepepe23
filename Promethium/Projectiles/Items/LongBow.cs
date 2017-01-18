using System;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Promethium.Projectiles.Items
{
    public class LongBow : AnimItem
    {
        public override void SetDefaults(ref int frames, ref int animSpeed)
        {
            projectile.name = "Long Bow";
            frames = 6;
            animSpeed = 20;
            projectile.width = 30;
            projectile.height = 78;
        }

        public override void CustomAI()
        {
            UpdateRotation();
            Player plr = Main.player[projectile.owner];
            Item ammoSlot = null;
            int speed = (int)Math.Floor(projectile.frame / 2f) + 1; //change this var name maybe?
            for (int i = 0; i < 58; i++)
            {
                if (plr.inventory[i].ammo == AmmoID.Arrow && plr.inventory[i].stack > 0 && plr.inventory[i].type != mod.ItemType("LongBow"))
                {
                    ammoSlot = plr.inventory[i];
                    break;
                }
            }
            if (plr.releaseUseItem /*&& projectile.frame > 3*/)
            {
                projectile.damage *= projectile.frame > 3 ? 4 : speed;
                ShootProjectile(ammoSlot.shoot, speed * 2, SoundID.Item5);
                ammoSlot.stack -= ammoSlot.consumable ? 1 : 0;
            }
            plr.itemRotation -= plr.direction * MathHelper.PiOver4;
        }

        public override void Animate(int speed)
        {
            int mod = speed;
            if (projectile.frame > 3)
                mod -= 15;
            if (++projectile.frameCounter >= mod)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame == 6)
                    projectile.frame = 4;
            }
        }
    }
}

