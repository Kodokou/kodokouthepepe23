using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Promethium.Items.Weapons
{
    class QuantumBreak : ModItem
    {
        private Rectangle hitBox = new Rectangle();
        private int resetTimer = 0;
        private float feetY = 0;

        public override void SetDefaults()
        {
            item.name = "Quantum Break";
            item.toolTip = "Allows to break the passage of time itself";
            item.damage = 115;
            item.width = 60;
            item.height = 60;
            item.melee = true;
            item.useStyle = 1;
            item.useTime = 20;
            item.useAnimation = 20;
            item.autoReuse = false;
            item.knockBack = 5;
            item.value = Item.buyPrice(0, 75);
            item.rare = 8;
            item.UseSound = SoundID.Item15;
            item.scale = 1.15F;
        }

        public override bool AltFunctionUse(Player plr)
        {
            if (plr.ownedProjectileCounts[mod.ProjectileType<Projectiles.ChronoTag>()] > 0)
            {
                int shortestTimeLeft = int.MaxValue;
                Projectile shortest = Main.projectile[0];
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.type == mod.ProjectileType<Projectiles.ChronoTag>() && p.owner == plr.whoAmI && p.timeLeft < shortestTimeLeft)
                    {
                        shortestTimeLeft = p.timeLeft;
                        shortest = p;
                    }
                }
                if (plr.mount.Active) plr.QuickMount();
                Utils.RegenEffect(plr, 229);
                plr.Center = shortest.Center - Vector2.UnitX * plr.direction * 80;
                plr.velocity /= 3;
                plr.velocity.X += 7 * plr.direction;
                Utils.RegenEffect(plr, 229);
                item.useStyle = 3;
                shortest.Kill();
                return true;
            }
            return false;
        }

        public override void UseStyle(Player plr)
        {
            resetTimer = 20;
            if (item.useStyle == 2)
            {
                plr.itemLocation.X -= plr.direction * 24;
                plr.itemLocation.Y -= 16;
                plr.itemRotation = plr.itemRotation * 0.85F + plr.direction;
                if (plr.itemAnimation == 24)
                {
                    feetY = plr.position.Y + plr.height;
                    plr.velocity.Y -= plr.gravDir * Player.jumpSpeed * 2;
                }
                else if (Main.myPlayer == plr.whoAmI)
                {
                    if (plr.itemAnimation % 4 == 1)
                    {
                        Vector2 pos = new Vector2(plr.Center.X + plr.direction * (30 - plr.itemAnimation) * 9, feetY - 24);
                        Projectile.NewProjectile(pos, plr.velocity + new Vector2(plr.direction, -2), mod.ProjectileType<Projectiles.QuantumBeam>(), item.damage, item.knockBack, plr.whoAmI);
                    }
                }
            }
            if (plr.itemAnimation == 1)
            {
                if (item.useStyle == 3)
                {
                    item.useStyle = 2;
                    item.useAnimation = 25;
                }
                else
                {
                    if (item.useStyle == 1) item.useStyle = 3;
                    else item.useStyle = 1;
                    item.useAnimation = 20;
                }
            }
            for (int i = 0; i < 2; ++i)
            {
                Dust d = Main.dust[Dust.NewDust(new Vector2(hitBox.X, hitBox.Y), hitBox.Width, hitBox.Height, 229, plr.velocity.X * 0.2f + plr.direction * 3, plr.velocity.Y * 0.2f, 192)];
                d.noGravity = true;
                d.velocity *= 2;
            }
        }

        public override void UseItemHitbox(Player plr, ref Rectangle hitbox, ref bool noHitbox)
        {
            if (item.useStyle == 2)
            {
                hitbox.X += plr.direction * 16;
                hitbox.Y -= 16;
            }
            hitBox = hitbox;
        }

        public override void OnHitNPC(Player plr, NPC target, int damage, float knockBack, bool crit)
        {
            if (item.useStyle == 2) Utils.CustomKnockback(target, knockBack, 0, -2);
        }

        public override void UpdateInventory(Player plr)
        {
            if (resetTimer > 0 && --resetTimer == 0)
            {
                item.useStyle = 1;
                item.useAnimation = 20;
            }
        }
    }
}
