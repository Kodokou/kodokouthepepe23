using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
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
            if (player.FindBuffIndex(mod.BuffType<Buffs.ManaBuckler>()) == -1)
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

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (manaBucklerLeft > 0)
            {
                int dmg = (int)(Main.CalculatePlayerDamage(damage, player.statDefense) * 1.5F);
                if (player.CheckMana(dmg, true, true))
                {
                    player.manaRegenDelay = (int)player.maxRegenDelay;
                    if (--manaBucklerLeft < 1)
                    {
                        int buff = player.FindBuffIndex(mod.BuffType<Buffs.ManaBuckler>());
                        if (buff != -1) player.buffTime[buff] = 0;
                    }
                    player.immune = true;
                    if (pvp) player.immuneTime = 8;
                    else
                    {
                        player.immuneTime = 20;
                        if (dmg > 1) player.immuneTime *= 2;
                        if (player.longInvince) player.immuneTime *= 2;
                    }
                    Main.PlaySound(SoundID.Item27, player.position);
                    double maxDust = dmg / player.statManaMax2 * 100;
                    int i = 0;
                    while (i++ < maxDust) Dust.NewDust(player.position, player.width, player.height, 5, hitDirection * 2, -2, 0, Color.Blue);
                    if (!player.noKnockback && hitDirection != 0 && (!player.mount.Active || !player.mount.Cart))
                    {
                        player.velocity.X = 4.5f * hitDirection;
                        player.velocity.Y = -3.5f;
                    }
                    return false;
                }
            }
            return true;
        }

        public override void ModifyDrawLayers(System.Collections.Generic.List<PlayerLayer> layers)
        {
            frontLayer.visible = true;
            layers.Add(frontLayer);
        }

        public override void PostUpdate()
        {
            if (++time >= 128) time = 0;
        }

        public static readonly PlayerLayer frontLayer = new PlayerLayer("Promethium", "CCM Front Layer", drawInfo =>
        {
            Mod mod = ModLoader.GetMod("Promethium");
            Player plr = drawInfo.drawPlayer;
            CCMPlayer mplr = plr.GetModPlayer<CCMPlayer>(mod);
            if (mplr.manaBucklerLeft > 0)
            {
                Texture2D tex = mod.GetTexture("Items/Weapons/ManaBuckler");
                Vector2 pos = plr.MountedCenter + new Vector2(16 * plr.direction, 4);
                float step = System.Math.Abs(mplr.time - 64) / 64F;
                float scale = step * 0.3F + 0.85F;
                Color c = Lighting.GetColor((int)(pos.X / 16F), (int)(pos.Y / 16F)) * (step * 0.5F + 0.25F);
                c.B = (byte)(c.B > 205 ? 255 : c.B + 50);
                pos -= Main.screenPosition;
                DrawData data = new DrawData(tex, pos, null, c, 0, tex.Size() / 2, scale, SpriteEffects.None, 0);
                Main.playerDrawData.Add(data);

                // TODO: Maybe some effects IDK, that's how you do them tho
                //int dust = Dust.NewDust();
                //Main.playerDrawDust.Add(dust); // Important apparently
            }
            
            /* Custom pseudo-accessory drawing test
            if (plr.HeldItem != null)
            {
                int body = plr.body;
                DrawData drawData;
                Vector2 pos = drawInfo.position;
                pos = new Vector2((int)(pos.X - Main.screenPosition.X - plr.bodyFrame.Width / 2 + plr.width / 2), (int)(pos.Y - Main.screenPosition.Y + plr.height - plr.bodyFrame.Height + 4)) + plr.bodyPosition + new Vector2(plr.bodyFrame.Width / 2, plr.bodyFrame.Height / 2);
                Texture2D tex = mod.GetTexture("Projectiles/Items/ShortCrossbow");
                drawData = new DrawData(tex, pos, null, drawInfo.bodyColor, plr.bodyRotation, drawInfo.bodyOrigin, 1, drawInfo.spriteEffects, 0);
                Main.playerDrawData.Add(drawData);
            }
            */
        });
    }
}