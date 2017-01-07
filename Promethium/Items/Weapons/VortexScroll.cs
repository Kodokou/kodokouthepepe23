using System;
using Terraria.ModLoader;
using Promethium.Projectiles;

namespace Promethium.Items
{
	public class VortexScroll : ModItem
	{
		public override void SetDefaults ()
		{
			item.damage = 50;
			item.name = "Vortex Scroll"; //change this to whatever
			item.Size = Vector2.One * 32;
			item.toolTip = "Summons a vortex that moves around and sucks in nearby enemies"; //change this to whatever
			item.shoot = mod.ProjectileType<Vortex> ();
			item.shootSpeed = 1;
			item.magic = true;
			item.useStyle = 3;
			item.useTime = 10;
			item.useAnimation = 10;
			item.mana = 1; //for testing
		}
	}
}

