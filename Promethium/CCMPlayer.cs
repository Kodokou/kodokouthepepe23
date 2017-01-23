using System;
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
        public float statNecro = 0;
        public bool necroEq = false;

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
            if (player.FindBuffIndex(mod.BuffType<Buffs.Necromancer>()) == -1)
                statNecro = 0;
            // TODO: Based on Necromancer armor set?
            necroEq = true;
        }

        private void KeyDoubleTap(int key)
        {
            if (key == (Main.ReversedUpDownArmorSetBonuses ? KEY_UP : KEY_DOWN))
                if (necroEq) player.MinionNPCTargetAim();
        }

        private void KeyHoldDown(int key, int time)
        {
            if (key == (Main.ReversedUpDownArmorSetBonuses ? KEY_UP : KEY_DOWN))
                if (necroEq && time >= 60) player.MinionAttackTargetNPC = -1;
        }

        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (necroEq)
            {
                if (target.life <= 0) statNecro += target.lifeMax / 50F;
                if (statNecro >= 1) player.AddBuff(mod.BuffType<Buffs.Necromancer>(), int.MaxValue, false);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            OnHitNPC(null, target, damage, knockback, crit);
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
            backLayer.visible = true;
            layers.Insert(0, backLayer);
        }

        public override void PostUpdate()
        {
            if (++time >= 256) time = 0;
        }

        public static readonly PlayerLayer frontLayer = new PlayerLayer("Promethium", "CCM Front Layer", drawInfo =>
        {
            Mod mod = ModLoader.GetMod("Promethium");
            Player plr = drawInfo.drawPlayer;
            CCMPlayer mplr = plr.GetModPlayer<CCMPlayer>(mod);
            if (drawInfo.shadow == 0 && mplr.manaBucklerLeft > 0)
            {
                Texture2D tex = mod.GetTexture("Items/Weapons/ManaBuckler");
                Vector2 pos = plr.MountedCenter + new Vector2(16 * plr.direction, 4);
                float step = System.Math.Abs((mplr.time % 128) - 64) / 64F;
                float scale = step * 0.3F + 0.85F;
                Color c = Lighting.GetColor((int)(pos.X / 16F), (int)(pos.Y / 16F)) * (step * 0.5F + 0.25F);
                c.B = (byte)(c.B > 205 ? 255 : c.B + 50);
                pos -= Main.screenPosition;
                DrawData data = new DrawData(tex, pos, null, c, 0, tex.Size() / 2, scale, SpriteEffects.None, 0);
                Main.playerDrawData.Add(data);
            }
            /* Custom accessory drawing test
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
            if (drawInfo.shadow == 0) DrawBones(plr, 1);
        });

        public static readonly PlayerLayer backLayer = new PlayerLayer("Promethium", "CCM Back Layer", drawInfo =>
        {
            if (drawInfo.shadow == 0) DrawBones(drawInfo.drawPlayer, -1);
        });

        private static void DrawBones(Player plr, int sideMult)
        {
            CCMPlayer mplr = plr.GetModPlayer<CCMPlayer>(ModLoader.GetMod("Promethium"));
            if (mplr.statNecro > 4)
            {
                int maxI = (int)Math.Log(mplr.statNecro);
                Texture2D tex = Main.itemTexture[ItemID.SkullLantern];
                for (int i = maxI; i > 0; --i)
                {
                    float normTime;
                    if (i % 3 == 0) normTime = MathHelper.Pi * (128 - mplr.time) * 3 / 64 + i * MathHelper.TwoPi * 3 / maxI;
                    else normTime = MathHelper.Pi * (128 - mplr.time) * 3 / 128 + i * MathHelper.TwoPi * 3 / maxI;
                    float lside = ((Math.Abs(normTime) + MathHelper.PiOver2) % MathHelper.TwoPi) - MathHelper.Pi;
                    if ((sideMult == 1 && lside >= 0) || (sideMult == -1 && lside < 0))
                    {
                        Vector2 pos = plr.MountedCenter;
                        pos += new Vector2((float)Math.Sin(normTime) * plr.width * 5 / 4, (float)Math.Cos(normTime / 3) * plr.height * 5 / 8);
                        DrawData data = new DrawData(tex, pos - Main.screenPosition, null, Lighting.GetColor((int)(pos.X / 16), (int)(pos.Y / 16)) * 0.75F, 0, tex.Size() / 2, 0.75F, lside > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                        Main.playerDrawData.Add(data);
                        if (Main.rand.Next(4) == 0)
                        {
                            int d = Dust.NewDust(pos - new Vector2(4, 4), tex.Width, tex.Height, 65, 0, 0, 92, default(Color), 0.9F);
                            Main.playerDrawDust.Add(d);
                        }
                    }
                }
            }
        }
    }
}