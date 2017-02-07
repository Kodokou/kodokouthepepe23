using Terraria.ModLoader;

namespace Promethium
{
	class ContraCorruptamMundi : Mod
	{
		public ContraCorruptamMundi()
		{
            Properties = new ModProperties() { Autoload = true };
		}

        public override void ChatInput(string text, ref bool broadcast)
        {
            if (text.Equals("/newaitest", System.StringComparison.OrdinalIgnoreCase))
            {
                Projectiles.Minions.Skeleton.NEW_AI_TEST = !Projectiles.Minions.Skeleton.NEW_AI_TEST;
                broadcast = false;
                Terraria.Main.NewText("New AI test " + (Projectiles.Minions.Skeleton.NEW_AI_TEST ? "en" : "dis") + "abled");
            }
        }
    }
}
