using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Promethium.Items.Weapons
{
    public class BladeWorks : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 1050;
            item.name = "Blade Works";
            item.width = 30;
            item.height = 34;
            item.useStyle = 5;
            item.useAnimation = 22;
            item.useTime = 22;
            item.magic = true;
            item.value = Item.buyPrice(0, 2);
            item.mana = 9;
            item.rare = 3;
            item.noMelee = true;
            item.toolTip = "'I am the Bone of my Sword...'";
            item.UseSound = SoundID.Item8;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player p = Main.player[Main.myPlayer];
            int dmgBoost = p.GetWeaponDamage(item) / 10 - 100;
            string dmgText = (dmgBoost < 0 ? "" : "+") + dmgBoost;
            for (int i = tooltips.Count - 1; i >= 0; --i)
            {
                TooltipLine line = tooltips[i];
                if (line.Name == "Damage") line.text = "Projection " + dmgText + "% magic damage";
                else if (line.Name == "Knockback") line.text = "Projection knockback";
            }
        }

        public override bool UseItem(Player plr)
        {
            if (Main.myPlayer == plr.whoAmI)
            {
                Vector2 targetPos = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
                plr.ChangeDir(targetPos.X < plr.Center.X ? -1 : 1);
                for (int i = 0; i < 2; ++i)
                {
                    Vector2 spawnPos = plr.Center - new Vector2(Main.rand.Next(25, 250) * plr.direction, 150);
                    spawnPos.Y -= 25 * i;
                    Vector2 speed = targetPos - spawnPos;
                    speed.Normalize();
                    speed *= 16;
                    speed.Y += Main.rand.Next(-40, 41) * 0.0125F;
                    Item temp = new Item();
                    temp.SetDefaults(GetItemID());
                    Projectile.NewProjectile(spawnPos, speed, mod.ProjectileType<Projectiles.BladeProjection>(), plr.GetWeaponDamage(temp) * plr.GetWeaponDamage(item) / 1000, plr.GetWeaponKnockback(temp, temp.knockBack), plr.whoAmI, temp.type);
                }
            }
            return true;
        }

        private static int GetItemID()
        {
            int i = Main.rand.Next(126);
            if (i == 125) return ItemID.Excalibur;
            else i /= 5;
            if (i == 24) return ItemID.EnchantedSword;
            else i %= 6;
            if (i == 0) return ItemID.LeadBroadsword;
            else if (i == 1) return ItemID.SilverBroadsword;
            else if (i == 2) return ItemID.GoldBroadsword;
            else if (i == 3) return ItemID.PlatinumBroadsword;
            else if (i == 4) return ItemID.TungstenBroadsword;
            else return ItemID.PearlwoodSword;
        }
    }
}