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
            item.damage = 1000;
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
            item.toolTip = "'I am the bone of my sword...'";
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
            Vector2 targetPos = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY);
            plr.direction = targetPos.X < plr.Center.X ? -1 : 1;
            float ai0 = targetPos.Y;
            if (ai0 > plr.Center.Y - 200) ai0 = plr.Center.Y - 200;
            for (int i = 0; i < 2; ++i)
            {
                Vector2 spawnPos = plr.Center - new Vector2(Main.rand.Next(25, 250) * plr.direction, 150);
                spawnPos.Y -= 25 * i;
                Vector2 speed = targetPos - spawnPos;
                speed.Normalize();
                speed *= 12;
                speed.Y += Main.rand.Next(-40, 41) * 0.0125F;
                Item temp = new Item();
                temp.SetDefaults(GetItemID());
                Projectile.NewProjectile(spawnPos, speed, mod.ProjectileType("BladeProjection"), plr.GetWeaponDamage(temp) * item.damage / 1000, plr.GetWeaponKnockback(temp, temp.knockBack), plr.whoAmI, ai0, temp.type);
            }
            return true;
        }

        private static int GetItemID()
        {
            int i = Main.rand.Next(125);
            if (i == 124) return ItemID.Excalibur;
            else i /= 5;
            if (i == 24) return ItemID.LightsBane;
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