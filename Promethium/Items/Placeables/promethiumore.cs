using Terraria.ModLoader;

namespace Promethium.Items.Placeable
{
    public class PromethiumOre : ModItem
    {
        public override void SetDefaults()
        {
            item.name = "Promethium Ore";
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = mod.TileType("PromethiumOre");
        }
    }
}