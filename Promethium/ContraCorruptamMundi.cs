using Terraria.ModLoader;

namespace Promethium
{
    class ContraCorruptamMundi : Mod
    {
        public ContraCorruptamMundi()
        {
            Properties = new ModProperties
            {
                Autoload = true
            };
        }
    }

    class AiTestCommand : ModCommand
    {
        public override string Command => "newaitest";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Projectiles.Minions.Skeleton.NEW_AI_TEST = !Projectiles.Minions.Skeleton.NEW_AI_TEST;
            Terraria.Main.NewText("New AI test " + (Projectiles.Minions.Skeleton.NEW_AI_TEST ? "en" : "dis") + "abled");
        }
    }
}
