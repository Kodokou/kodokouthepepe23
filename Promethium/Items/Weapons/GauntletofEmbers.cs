using Terraria;
using Terraria.ModLoader;

namespace Promethium.Items.Weapons
{
    class GauntletofEmbers : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gauntlet of Embers");
            Tooltip.SetDefault("'My hand feels like it is melting!'");
        }

        public override void SetDefaults()
        {
            item.damage = 20;
            item.width = 34;
            item.height = 34;
            item.channel = true;
            item.magic = true;
            item.mana = 15;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.value = Item.buyPrice(0, 2);
            item.rare = 5;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shoot = mod.ProjectileType<Projectiles.Items.GauntletofEmbers>();
            item.shootSpeed = 14;
        }
    }
}
