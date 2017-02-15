using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;

namespace Promethium.Items.Weapons
{
	public class VoidOrb : ModItem
	{
		public override void SetDefaults ()
		{
			item.damage = 22;
			item.name = "Void Orb";
            item.width = 30;
            item.height = 30;
			item.toolTip = "Spawns a vortex that draws in nearby enemies";
			item.shoot = mod.ProjectileType<Projectiles.VoidOrb>();
			item.shootSpeed = 5;
			item.summon = true;
			item.useStyle = 2;
			item.useTime = 24;
			item.useAnimation = 24;
            item.mana = 50;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.autoReuse = false;
            item.knockBack = 6;
            item.value = Item.buyPrice(0, 2);
            item.rare = 6;
            item.UseSound = Terraria.ID.SoundID.Item1;
        }

        public override DrawAnimation GetAnimation()
        {
            return new DrawAnimationVertical(10, 3);
        }
    }
}

