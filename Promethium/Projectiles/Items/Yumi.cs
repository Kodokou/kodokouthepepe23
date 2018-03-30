using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Promethium.Projectiles.Items
{
    public class Yumi : AnimItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yumi");
        }

        public override void SetDefaults(ref int frames, ref int animSpeed)
        {
            frames = 6;
            animSpeed = 15;
            projectile.width = 30;
            projectile.height = 78;
            rotShift = 0;
        }

        public override void CustomAI()
        {
            UpdateRotation();
            if (Main.myPlayer == projectile.owner)
            {
                Player plr = Main.player[projectile.owner];
                if (!plr.mount.Active)
                {
                    plr.velocity.X *= 0.9F;
                    if (plr.velocity.Y < 0) plr.velocity.Y *= 0.9F;
                }
            }
        }

        public override void Animate()
        {
            int mod = animSpeed;
            if (projectile.frame > 3) mod += 45;
            if (++projectile.frameCounter >= mod)
            {
                projectile.frameCounter = 0;
                if (projectile.frame < 5 && ++projectile.frame > 3) Utils.RegenEffect(Main.player[projectile.owner]);
            }
            if (projectile.frame > 4 && projectile.frameCounter % 3 == 0)
            {
                Vector2 v = Main.rand.NextVector2CircularEdge(16, 16);
                Vector2 pv = Main.player[projectile.owner].velocity * 2 / 3;
                Dust d = Main.dust[Dust.NewDust(Main.player[projectile.owner].MountedCenter + projectile.velocity * 2 + v, 1, 1, 127, -v.X / 2 + pv.X, -v.Y + pv.Y / 2, 96, default(Color), 1.25F)];
                d.noGravity = true;
                d.noLight = true;
            }
        }

        public override void Action()
        {
            if (projectile.frame > 3)
            {
                Item it = new Item() { type = mod.ItemType<Promethium.Items.Weapons.Yumi>(), useAmmo = AmmoID.Arrow };
                int shoot = ProjectileID.WoodenArrowFriendly;
                float speed = 14;
                bool canShot = false;
                Main.player[projectile.owner].PickAmmo(it, ref shoot, ref speed, ref canShot, ref projectile.damage, ref projectile.knockBack);
                if (projectile.frame > 4)
                {
                    speed = speed * 5 / 4;
                    projectile.knockBack = projectile.knockBack * 4 / 3;
                    projectile.damage = projectile.damage * 3 / 2;
                }
                if (canShot) ShootProjectile(shoot, speed, SoundID.Item5);
            }
            projectile.frame = 0;
        }
    }
}

