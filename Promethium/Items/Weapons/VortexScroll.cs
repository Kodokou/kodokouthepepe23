using Terraria.ModLoader;

namespace Promethium.Items
{
	public class VortexScroll : ModItem
	{
		public override void SetDefaults ()
		{
			item.damage = 50;
			item.name = "Void Orb";
            item.width = 32;
            item.height = 32;
			item.toolTip = "Spawns a vortex that sucks in nearby enemies";
			item.shoot = mod.ProjectileType("Vortex"); // Change to throwable projectile
			item.shootSpeed = 1;
			item.magic = true;
			item.useStyle = 3; // Change to throwable projectile
			item.useTime = 10;
			item.useAnimation = 10;
			item.mana = 50;
		}
	}
}

