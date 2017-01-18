using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace Promethium.Items.Weapons
{
    class FingerofZeus : ModItem
    {
        public override void SetDefaults()
        {
            item.name = "Finger of Zeus";
            item.damage = 70;
            item.magic = true;
            item.width = 40;
            item.height = 40;
            item.toolTip = "+50% damage during rain";
            item.useTime = 10;
            item.useAnimation = 20;
            item.useStyle = 5;
            Item.staff[item.type] = true;
            item.noMelee = true;
            item.knockBack = 2;
            item.value = Item.buyPrice(0, 70);
            item.rare = 8;
            item.noUseGraphic = true;
            item.channel = true;
            item.shoot = mod.ProjectileType<Projectiles.Items.FingerofZeus>();
            item.shootSpeed = 40;
        }

        public override DrawAnimation GetAnimation()
        {
            return new DrawAnimationVertical(10, 3);
        }
    }
}
