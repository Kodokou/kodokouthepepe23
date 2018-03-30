using Terraria.ID;
using Terraria.ModLoader;

namespace Promethium.Items.Weapons
{
    public class HeavyFlail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heavy Flail");
            Tooltip.SetDefault("Hits harder the faster you swing it around");
        }

        public override void SetDefaults()
        {
            item.noMelee = true;
            item.useStyle = 5;
            item.useAnimation = 45;
            item.useTime = 45;
            item.knockBack = 7;
            item.width = 34;
            item.height = 38;
            item.damage = 20;
            item.noUseGraphic = true;
            item.shoot = mod.ProjectileType<Projectiles.HeavyFlail>();
            item.shootSpeed = 9;
            item.UseSound = SoundID.Item1;
            item.rare = 2;
            item.value = 27000;
            item.melee = true;
            item.channel = true;
        }
    }
}

