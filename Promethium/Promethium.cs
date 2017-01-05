using Terraria.ModLoader;

namespace Promethium
{
	class Promethium : Mod
	{
		public Promethium()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}
	}
}
