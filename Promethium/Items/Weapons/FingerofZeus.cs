using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Promethium.Items.Weapons
{
    class FingerofZeus : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Finger of Zeus");
            Tooltip.SetDefault("+50% damage during rain");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(10, 3));
        }

        public override void SetDefaults()
        {
            item.damage = 70;
            item.magic = true;
            item.width = 40;
            item.height = 40;
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
    }
}
