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
            rotShift = 0;
        }

        public override void CustomAI()
        {
            UpdateRotation();
        }

        public override void Animate()
        {
            int mod = animSpeed;
            if (projectile.frame > 3) mod += 100;
            if (++projectile.frameCounter >= mod)
            {
                projectile.frameCounter = 0;
                if (projectile.frame < 5 && ++projectile.frame > 3)
                    Utils.RegenEffect(Main.player[projectile.owner]);
            }
        }

        public override void Action()
        {
            if (projectile.frame > 3)
            {
                Item it = new Item() { type = mod.ItemType<Promethium.Items.Weapons.LongBow>(), useAmmo = AmmoID.Arrow };
                int shoot = ProjectileID.WoodenArrowFriendly;
                float speed = 9;
                bool canShot = false;
                Main.player[projectile.owner].PickAmmo(it, ref shoot, ref speed, ref canShot, ref projectile.damage, ref projectile.knockBack);
                if (projectile.frame > 4)
                {
                    speed = speed * 4 / 3;
                    projectile.damage = projectile.damage * 4 / 3;
                    projectile.knockBack = projectile.knockBack * 4 / 3;
                }
                if (canShot) ShootProjectile(shoot, speed, SoundID.Item5);
            }
            projectile.frame = 0;
        }
    }
}

