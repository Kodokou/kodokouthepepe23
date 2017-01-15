using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Promethium
{
    class CCMPlayer : ModPlayer
    {
        private const byte KEY_DOWN = 0, KEY_UP = 1, KEY_RIGHT = 2, KEY_LEFT = 3;
        public int manaBucklerLeft = 0, time = 0;

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

        public override void ResetEffects()
        {
            if (player.FindBuffIndex(mod.BuffType("ManaBuckler")) == -1)
                manaBucklerLeft = 0;
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

        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            int buffIndex = player.FindBuffIndex(mod.BuffType("ManaBuckler"));
            if (player.statMana - damage * 2 >= 0 && manaBucklerLeft > 0)
            {
                manaBucklerLeft--;
                player.statMana -= damage * 2;
                player.statLife += damage;
            }
            if (player.statMana - damage * 2 < 0 || manaBucklerLeft < 1)
            {
                player.buffTime[buffIndex] = 0;
            }   
        }

        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            ModifyHitByNPC(null, ref damage, ref crit);
        }

        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (player.HeldItem.type == mod.ItemType("GauntletofEmbers"))
                drawInfo.drawHands = false;
        }

        public override void ModifyDrawLayers(System.Collections.Generic.List<PlayerLayer> layers)
        {
            frontLayer.visible = true;
            layers.Add(frontLayer);
        }

        public override void PostUpdate()
        {
            if (++time >= 256)
                time = 0;
        }

        public static readonly PlayerLayer frontLayer = new PlayerLayer("Promethium", "CCM Front Layer", PlayerLayer.MiscEffectsFront, drawInfo =>
            {
                
                Mod mod = ModLoader.GetMod("Promethium");
                Player plr = drawInfo.drawPlayer;
                CCMPlayer mplr = plr.GetModPlayer<CCMPlayer>(mod);
                if (mplr.manaBucklerLeft > 0)
                {
                    
                    Texture2D tex = mod.GetTexture("Items/Weapons/ManaBuckler");
                    Vector2 pos = plr.MountedCenter + Vector2.UnitX * 8 * plr.direction;
                    float step = System.Math.Abs(mplr.time - 128) / 128F;
                    float scale = step * 0.2F + 0.9F;
                    Color c = Lighting.GetColor((int)(pos.X / 16F), (int)(pos.Y / 16F)) * (step * 0.5F + 0.25F);
                    pos -=  Main.screenPosition;
                    var data = new Terraria.DataStructures.DrawData(tex, pos, null, c, 0, tex.Size() / 2 * scale, scale, SpriteEffects.None, 0);
                    Main.playerDrawData.Add(data);

                    // TODO: Maybe some effects IDK, that's how you do them tho
                    //int dust = Dust.NewDust();
                    //Main.playerDrawDust.Add(dust); // Important apparently
                }
            });
    }
}
