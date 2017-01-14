using Terraria;
using Terraria.ModLoader;

namespace Promethium.Items.Weapons
{
	public class FireGem : ModItem
	{
        public override void SetDefaults ()
        {
            item.damage = 42;
            item.name = "Fire Gems";
            item.width = 30;
            item.height = 34;
            item.toolTip = "Magical gems that explode when enemies get close to them";
            item.shoot = mod.ProjectileType("FireGem");
            item.shootSpeed = 15;
            item.summon = true;
            item.useStyle = 1;
            item.useTime = 24;
            item.useAnimation = 24;
            item.mana = 28;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.autoReuse = false;
            item.knockBack = 6;
            item.value = Item.buyPrice(0, 2);
            item.rare = 6;
            item.UseSound = Terraria.ID.SoundID.Item1;
        }
	}
}

