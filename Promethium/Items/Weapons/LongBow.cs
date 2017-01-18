using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Promethium.Items.Weapons
{
    public class LongBow : ModItem
    {
        public override void SetDefaults()
        {
            item.name = "Long Bow";
            item.damage = 20;
            item.width = 20;
            item.height = 58;
            item.damage = 24;
            item.knockBack = 1f;
            item.channel = true;
            item.ranged = true;
            item.useAmmo = AmmoID.Arrow;
            item.useTime = 20;
            item.useAnimation = 30;
            item.reuseDelay = 15;
            item.useStyle = 5;
            item.value = Item.buyPrice(0, 2);
            item.rare = 5;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.toolTip = "";
            item.shoot = mod.ProjectileType("LongBow");
            item.shootSpeed = 18;
        }

        public override bool ConsumeAmmo(Player player)
        {
            return false;
        }

        public override bool Shoot(Player plr, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = item.shoot;
            return base.Shoot(plr, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
        }

        public override bool CanUseItem(Player plr)
        {
            return plr.HasAmmo(item, false);
        }
    }
}

