using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Promethium.Tiles
{
    public class JellyOre : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = false;
            soundType = 27;
            soundStyle = 2;
            drop = mod.ItemType<Items.Placeables.JellyOre>();
            //AddMapEntry(new Color(142, 217, 63));
        }

        public override bool KillSound(int i, int j)
        {
            Main.PlaySound(4, i * 16, j * 16, 1);
            return false;
        }
    }
}