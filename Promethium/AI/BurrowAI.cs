using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Promethium.AI
{
    public class BurrowAI : BaseAI
    {
        public override bool AI(AIUser aiu)
        {
            float dist = float.MaxValue;
            Projectile aip = Main.projectile[aiu.entity.whoAmI];
            NPC pTarget = null;
            if (pTarget == null || !pTarget.active)
            {
                for (int i = 0; i < 200; ++i)
                {
                    NPC potNPC = Main.npc[i];
                    float potDist = potNPC.DistanceSQ(aip.position);
                    if (potNPC.active && potDist < dist)
                    {
                        pTarget = potNPC;
                        dist = potDist;
                    }
                }
            }
            /*float dist = int.MaxValue;
            Projectile aip = Main.projectile[aiu.entity.whoAmI];
            NPC pTarget = null;
            if (pTarget == null || !pTarget.active)
            {
                for (int i = 0; i < 200; ++i)
                {
                    NPC potNPC = Main.npc[i];
                    float potDist = potNPC.DistanceSQ(aip.position);
                    if (potNPC.active && potDist < dist)
                    {
                        pTarget = potNPC;
                        dist = potDist;
                    }
                }
            }
            int burrowX = (int)(aip.position.X / 16) - 1;
            int offsetBurrowX = (int)((aip.position.X + aiu.entity.width) / 16) + 2;
            int burrowY = (int)(aip.position.Y / 16) - 1;
            int offsetBurrowY = (int)((aip.position.Y + aiu.entity.height) / 16) + 2;
            float speed = 8f;
            float accel = 0.07f;
            Vector2 pCenter = new Vector2(aiu.entity.position.X + aiu.entity.width * 0.5f, aiu.entity.position.Y + aiu.entity.height * 0.5f);
            float targetCenterX = pTarget.position.X + pTarget.width / 2;
            float targetCenterY = pTarget.position.Y + pTarget.height / 2;
            float num17 = (int)(targetCenterX / 16) * 16;
            float num18 = (int)(targetCenterY / 16) * 16;
            pCenter.X = (int)(pCenter.X / 16) * 16; // so confused why this is here...
            pCenter.Y = (int)(pCenter.Y / 16) * 16; // this shouldn't do anything
            float distX = num17 - pCenter.X;
            float distY = num18 - pCenter.Y;
            for (int i = burrowX; i < offsetBurrowX; ++i)
            {
                for (int j = burrowY; j < offsetBurrowY; ++j)
                {
                    if (Main.tile[i, j] != null && (Main.tile[i, j].nactive() && (Main.tileSolid[Main.tile[i, j].type] || Main.tileSolidTop[Main.tile[i, j].type] && Main.tile[i, j].frameY == 0) || Main.tile[i, j].liquid > 64))
                    {
                        Vector2 vector2;
                        vector2.X = i * 16;
                        vector2.Y = j * 16;
                        if (aiu.entity.position.X + aiu.entity.width > vector2.X && aiu.entity.position.X < vector2.X + 16.0 && (aiu.entity.position.Y + aiu.entity.height > vector2.Y && aiu.entity.position.Y < vector2.Y + 16.0))
                        {
                            WorldGen.KillTile(i, j, true, true, false);
                        }
                    }
                }
            }
            if ((double)aip.ai[1] > 0.0)
            {
                if ((double)aip.ai[1] < (double)Main.npc.Length)
                {
                    try
                    {
                        pCenter = new Vector2(aip.position.X + aip.width * 0.5f, aip.position.Y + aip.height * 0.5f);
                        distX = Main.npc[(int)aip.ai[1]].position.X + (Main.npc[(int)aip.ai[1]].width / 2) - pCenter.X;
                        distY = Main.npc[(int)aip.ai[1]].position.Y + (Main.npc[(int)aip.ai[1]].height / 2) - pCenter.Y;
                    }
                    catch
                    {
                    }
                    aip.rotation = (float)Math.Atan2((double)distY, (double)distX) + MathHelper.PiOver2;
                    dist = (float)Math.Sqrt((double)distX * (double)distX + (double)distY * (double)distY); // optimizable to entity.distance? maybe?
                    float num12 = (dist - (float)aip.width) / dist;
                    float num13 = distX * num12;
                    float num14 = distY * num12;
                    aip.velocity = Vector2.Zero;
                    aip.position.X += num13;
                    aip.position.Y += num14;
                }
            }*/

            if (!head && aip.timeLeft < 300)
            {
                aip.timeLeft = 300;
            }
            if (pTarget == null || !pTarget.active)
            {
                for (int i = 0; i < 200; ++i)
                {
                    NPC potNPC = Main.npc[i];
                    float potDist = potNPC.DistanceSQ(aip.position);
                    if (potNPC.active && potDist < dist)
                    {
                        pTarget = potNPC;
                        dist = potDist;
                    }
                }
            }
            if (!pTarget.active && aip.timeLeft > 300)
            {
                aip.timeLeft = 300;
            }
            if (Main.netMode != 1)
            {
                if (!tail && aip.ai[0] == 0f)
                {
                    Vector2 center = new Vector2((int)(aip.position.X + (float)(aip.width / 2)), (int)(aip.position.Y + (float)aip.height));
                    if (head)
                    {
                        //aip.ai[3] = (float)aip.whoAmI;
                        aip.ai[1] = (float)Main.rand.Next(minLength, maxLength + 1);
                        aip.ai[0] = Projectile.NewProjectile(center, Vector2.Zero, bodyType, aip.damage, aip.knockBack);
                    }
                    else if (aip.ai[2] > 0f)
                    {
                        aip.ai[0] = Projectile.NewProjectile(center, Vector2.Zero, npc.type/*is bodytype*/, aip.damage, aip.knockBack);
                    }
                    else
                    {
                        aip.ai[0] = (float)NPC.NewNPC(center, tailType, npc.whoAmI);
                    }
                    Main.projectile[(int)aip.ai[0]].ai[1] = aip.ai[1] - 1f;
                    aip.netUpdate = true;
                }
                //Projectile self = Main.projectile[aip.whoAmI];
                Projectile back = Main.projectile[(int)aip.ai[0]];
                if (!head && (!aip.active || (aip.type != headType && aip.type != bodyType)))
                {
                    aip.active = false;
                }
                if (!tail && (!back.active || (back.type != bodyType && back.type != tailType)))
                {
                    npc.active = false;
                }
                if (!back.active && Main.netMode == 2)
                {
                    NetMessage.SendData(MessageID.KillProjectile, -1, -1, "", aip.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }
            }
            int num180 = (int)(aip.position.X / 16f) - 1;
            int num181 = (int)((aip.position.X + (float)aip.width) / 16f) + 2;
            int num182 = (int)(aip.position.Y / 16f) - 1;
            int num183 = (int)((aip.position.Y + (float)aip.height) / 16f) + 2;
            if (num180 < 0)
            {
                num180 = 0;
            }
            if (num181 > Main.maxTilesX)
            {
                num181 = Main.maxTilesX;
            }
            if (num182 < 0)
            {
                num182 = 0;
            }
            if (num183 > Main.maxTilesY)
            {
                num183 = Main.maxTilesY;
            }
            for (int num184 = num180; num184 < num181; num184++)
            {
                for (int num185 = num182; num185 < num183; num185++)
                {
                    if (Main.tile[num184, num185] != null && ((Main.tile[num184, num185].nactive() && (Main.tileSolid[(int)Main.tile[num184, num185].type] || (Main.tileSolidTop[(int)Main.tile[num184, num185].type] && Main.tile[num184, num185].frameY == 0))) || Main.tile[num184, num185].liquid > 64))
                    {
                        Vector2 vector17;
                        vector17.X = (float)(num184 * 16);
                        vector17.Y = (float)(num185 * 16);
                        if (npc.position.X + (float)npc.width > vector17.X && npc.position.X < vector17.X + 16f && npc.position.Y + (float)npc.height > vector17.Y && npc.position.Y < vector17.Y + 16f)
                        {
                            flag18 = true;
                            if (Main.rand.Next(100) == 0 && npc.behindTiles && Main.tile[num184, num185].nactive())
                            {
                                WorldGen.KillTile(num184, num185, true, true, false);
                            }
                            if (Main.netMode != 1 && Main.tile[num184, num185].type == 2)
                            {
                                ushort arg_BFCA_0 = Main.tile[num184, num185 - 1].type;
                            }
                        }
                    }
                }
            }
            bool flies;
            if (head)
            {
                Rectangle rectangle = new Rectangle((int)aip.position.X, (int)aip.position.Y, aip.width, aip.height);
                int num186 = 1000;
                bool flag19 = true;
                for (int num187 = 0; num187 < 255; num187++)
                {
                    if (Main.player[num187].active)
                    {
                        Rectangle rectangle2 = new Rectangle((int)Main.player[num187].position.X - num186, (int)Main.player[num187].position.Y - num186, num186 * 2, num186 * 2);
                        if (rectangle.Intersects(rectangle2))
                        {
                            flag19 = false;
                            break;
                        }
                    }
                }
                if (flag19)
                {
                    flies = true;
                }
            }

            float num188 = speed;
            float num189 = turnSpeed;
            Vector2 vector18 = new Vector2(aip.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num191 = pTarget.position.X + (float)(pTarget.width / 2);
            float num192 = pTarget.position.Y + (float)(pTarget.height / 2);
            num191 = (float)((int)(num191 / 16f) * 16);
            num192 = (float)((int)(num192 / 16f) * 16);
            vector18.X = (float)((int)(vector18.X / 16f) * 16);
            vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
            if (pTarget.ai[1] > 0f && pTarget.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(aip.position.X + (float)pTarget.width * 0.5f, aip.position.Y + (float)pTarget.height * 0.5f);
                    num191 = Main.npc[(int)aip.ai[0]].position.X + (float)(aip.width / 2) - vector18.X;
                    num192 = Main.npc[(int)aip.ai[0]].position.Y + (float)(aip.height / 2) - vector18.Y;
                }
                catch
                {
                }
                npc.rotation = (float)System.Math.Atan2((double)num192, (double)num191) + 1.57f;
                num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
                int num194 = aip.width;
                num193 = (num193 - (float)num194) / num193;
                num191 *= num193;
                num192 *= num193;
                aip.velocity = Vector2.Zero;
                aip.position.X = aip.position.X + num191;
                aip.position.Y = aip.position.Y + num192;
            }
            else
            {
                aip.velocity.Y = aip.velocity.Y + 0.11f;
                if (aip.velocity.Y > num188)
                {
                    aip.velocity.Y = num188;
                }
                if ((double)(System.Math.Abs(aip.velocity.X) + System.Math.Abs(aip.velocity.Y)) < (double)num188 * 0.4)
                {
                    if (aip.velocity.X < 0f)
                    {
                        aip.velocity.X = aip.velocity.X - num189 * 1.1f;
                    }
                    else
                    {
                        aip.velocity.X = aip.velocity.X + num189 * 1.1f;
                    }
                }
                else if (aip.velocity.Y == num188)
                {
                    if (npc.velocity.X < num191)
                    {
                        npc.velocity.X = npc.velocity.X + num189;
                    }
                    else if (npc.velocity.X > num191)
                    {
                        npc.velocity.X = npc.velocity.X - num189;
                    }
                }
                else if (npc.velocity.Y > 4f)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X + num189 * 0.9f;
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X - num189 * 0.9f;
                    }
                }
            }
            else
            {
                if (!flies && npc.behindTiles && npc.soundDelay == 0)
                {
                    float num195 = num193 / 40f;
                    if (num195 < 10f)
                    {
                        num195 = 10f;
                    }
                    if (num195 > 20f)
                    {
                        num195 = 20f;
                    }
                    npc.soundDelay = (int)num195;
                    Main.PlaySound(SoundID.Roar, npc.position, 1);
                }
                num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
                float num196 = System.Math.Abs(num191);
                float num197 = System.Math.Abs(num192);
                float num198 = num188 / num193;
                num191 *= num198;
                num192 *= num198;
                if (ShouldRun())
                {
                    bool flag20 = true;
                    for (int num199 = 0; num199 < 255; num199++)
                    {
                        if (Main.player[num199].active && !Main.player[num199].dead && Main.player[num199].ZoneCorrupt)
                        {
                            flag20 = false;
                        }
                    }
                    if (flag20)
                    {
                        if (Main.netMode != 1 && (double)(aip.position.Y / 16f) > (Main.rockLayer + (double)Main.maxTilesY) / 2.0)
                        {
                            npc.active = false;
                            int num200 = (int)aip.ai[0];
                            while (num200 > 0 && num200 < 200 && Main.npc[num200].active && Main.npc[num200].aiStyle == npc.aiStyle)
                            {
                                int num201 = (int)Main.npc[num200].ai[0];
                                Main.npc[num200].active = false;
                                npc.life = 0;
                                if (Main.netMode == 2)
                                {
                                    NetMessage.SendData(23, -1, -1, "", num200, 0f, 0f, 0f, 0, 0, 0);
                                }
                                num200 = num201;
                            }
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendData(23, -1, -1, "", npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                            }
                        }
                        num191 = 0f;
                        num192 = num188;
                    }
                }
                bool flag21 = false;
                if (npc.type == 87)
                {
                    if (((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f) || (npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)) && System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y) > num189 / 2f && num193 < 300f)
                    {
                        flag21 = true;
                        if (System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y) < num188)
                        {
                            npc.velocity *= 1.1f;
                        }
                    }
                    if (npc.position.Y > Main.player[npc.target].position.Y || (double)(Main.player[npc.target].position.Y / 16f) > Main.worldSurface || Main.player[npc.target].dead)
                    {
                        flag21 = true;
                        if (System.Math.Abs(npc.velocity.X) < num188 / 2f)
                        {
                            if (npc.velocity.X == 0f)
                            {
                                npc.velocity.X = npc.velocity.X - (float)npc.direction;
                            }
                            npc.velocity.X = npc.velocity.X * 1.1f;
                        }
                        else
                        {
                            if (npc.velocity.Y > -num188)
                            {
                                npc.velocity.Y = npc.velocity.Y - num189;
                            }
                        }
                    }
                }
                if (!flag21)
                {
                    if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
                    {
                        if (npc.velocity.X < num191)
                        {
                            npc.velocity.X = npc.velocity.X + num189;
                        }
                        else
                        {
                            if (npc.velocity.X > num191)
                            {
                                npc.velocity.X = npc.velocity.X - num189;
                            }
                        }
                        if (npc.velocity.Y < num192)
                        {
                            npc.velocity.Y = npc.velocity.Y + num189;
                        }
                        else
                        {
                            if (npc.velocity.Y > num192)
                            {
                                npc.velocity.Y = npc.velocity.Y - num189;
                            }
                        }
                        if ((double)System.Math.Abs(num192) < (double)num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
                        {
                            if (npc.velocity.Y > 0f)
                            {
                                npc.velocity.Y = npc.velocity.Y + num189 * 2f;
                            }
                            else
                            {
                                npc.velocity.Y = npc.velocity.Y - num189 * 2f;
                            }
                        }
                        if ((double)System.Math.Abs(num191) < (double)num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
                        {
                            if (npc.velocity.X > 0f)
                            {
                                npc.velocity.X = npc.velocity.X + num189 * 2f;
                            }
                            else
                            {
                                npc.velocity.X = npc.velocity.X - num189 * 2f;
                            }
                        }
                    }
                    else
                    {
                        if (num196 > num197)
                        {
                            if (npc.velocity.X < num191)
                            {
                                npc.velocity.X = npc.velocity.X + num189 * 1.1f;
                            }
                            else if (npc.velocity.X > num191)
                            {
                                npc.velocity.X = npc.velocity.X - num189 * 1.1f;
                            }
                            if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
                            {
                                if (npc.velocity.Y > 0f)
                                {
                                    npc.velocity.Y = npc.velocity.Y + num189;
                                }
                                else
                                {
                                    npc.velocity.Y = npc.velocity.Y - num189;
                                }
                            }
                        }
                        else
                        {
                            if (npc.velocity.Y < num192)
                            {
                                npc.velocity.Y = npc.velocity.Y + num189 * 1.1f;
                            }
                            else if (npc.velocity.Y > num192)
                            {
                                npc.velocity.Y = npc.velocity.Y - num189 * 1.1f;
                            }
                            if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
                            {
                                if (npc.velocity.X > 0f)
                                {
                                    npc.velocity.X = npc.velocity.X + num189;
                                }
                                else
                                {
                                    npc.velocity.X = npc.velocity.X - num189;
                                }
                            }
                        }
                    }
                }
                aip.rotation = (float)System.Math.Atan2((double)aip.velocity.Y, (double)aip.velocity.X) + 1.57f;
                if (head)
                {
                    if (npc.localAI[0] != 0f)
                    {
                        npc.netUpdate = true;
                    }
                    npc.localAI[0] = 0f;
                    
                    if (((aip.velocity.X > 0f && aip.oldVelocity.X < 0f) || (aip.velocity.X < 0f && aip.oldVelocity.X > 0f) || (aip.velocity.Y > 0f && aip.oldVelocity.Y < 0f) || (aip.velocity.Y < 0f && aip.oldVelocity.Y > 0f)))
                    {
                        aip.netUpdate = true;
                        return false;
                    }
                }
            }
            CustomBehavior();
            return true;
        }

        public override bool CanStart(AIUser aiu)
        {
            return true;
        }
    }
}
