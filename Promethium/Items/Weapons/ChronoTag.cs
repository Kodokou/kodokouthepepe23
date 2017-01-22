using Terraria.ModLoader;

namespace Promethium.Items.Weapons
{
    class ChronoTag : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 15;
            item.name = "Chrono Tag";
            item.width = 34;
            item.height = 34;
            item.toolTip = "Saves your current state in time";
            item.shoot = mod.ProjectileType<Projectiles.ChronoTag>();
            item.shootSpeed = 1;
            item.summon = true;
            item.useStyle = 4;
            item.useTime = 24;
            item.useAnimation = 24;
            item.mana = 99;
            item.noMelee = true;
            item.autoReuse = false;
            item.knockBack = 0;
            item.value = Terraria.Item.buyPrice(0, 75);
            item.rare = 6;
            item.UseSound = Terraria.ID.SoundID.Item8;
        }
    }
}
