using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Promethium.Items.Weapons
{
    class GauntletofEmbers : ModItem
    {
        public override void SetDefaults()
        {
            item.name = "Gauntlet of Embers";
            item.damage = 20;
            item.width = 34;
            item.height = 34;
            item.channel = true;
            item.magic = true;
            item.mana = 15;
            item.useTime = 20;
            item.useAnimation = 30;
            item.reuseDelay = 15;
            item.useStyle = 5;
            item.value = Item.buyPrice(0, 2);
            item.rare = 5;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.toolTip = "'My hands feel like they are melting!'";
            item.shoot = mod.ProjectileType("GauntletofEmbers");
            item.shootSpeed = 18;
        }
    }
}
