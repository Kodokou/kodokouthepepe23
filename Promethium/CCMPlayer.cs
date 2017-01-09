using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace Promethium
{
    class CCMPlayer : ModPlayer
    {
        private const byte KEY_DOWN = 0, KEY_UP = 1, KEY_RIGHT = 2, KEY_LEFT = 3;

        public override void SetControls()
        {
            for (int i = 0; i < 4; ++i)
            {
                bool released = (i == KEY_DOWN && player.controlDown && player.releaseDown) || (i == KEY_UP && player.controlUp && player.releaseUp);
                released |= (i == KEY_RIGHT && player.controlRight && player.releaseRight) || (i == KEY_LEFT && player.controlLeft && player.releaseLeft);
                bool pressed = (i == KEY_DOWN && player.controlDown) || (i == KEY_UP && player.controlUp);
                pressed |= (i == KEY_RIGHT && player.controlRight) || (i == KEY_LEFT && player.controlLeft);
                if (released && player.doubleTapCardinalTimer[i] > 0) KeyDoubleTap(i);
                if (pressed) player.KeyHoldDown(i, player.holdDownCardinalTimer[i]);
            }
        }

        private void KeyDoubleTap(int key)
        {
            //if (key == (Main.ReversedUpDownArmorSetBonuses ? KEY_UP : KEY_DOWN))
            //    player.MinionRestTargetAim();
        }

        private void KeyHoldDown(int key, int time)
        {
            //if (key == (Main.ReversedUpDownArmorSetBonuses ? KEY_UP : KEY_DOWN))
            //    if (time >= 60) player.MinionRestTargetPoint = Vector2.Zero;
        }
    }
}
