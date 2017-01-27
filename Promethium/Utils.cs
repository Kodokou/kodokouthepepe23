using Terraria;
using Terraria.ID;
using System;
using Microsoft.Xna.Framework;

namespace Promethium
{
    public static class Utils
    {
        public static void NetStrikeNPC(this NPC npc, int damage, float knockBack, int hitDirection, bool crit = false)
        {
            npc.StrikeNPC(damage, knockBack, hitDirection, crit, false, false);
            if (Main.netMode != 0) NetMessage.SendData(28, -1, -1, "", npc.whoAmI, damage, knockBack, hitDirection, crit ? 1 : 0);
        }

        public static bool IsSolid(this Tile t)
        {
            return t != null && t.nactive() && (Main.tileSolid[t.type] || (Main.tileSolidTop[t.type] && t.frameY == 0));
        }

        public static bool IsTopSolid(this Tile t)
        {
            return t != null && t.nactive() && Main.tileSolidTop[t.type] && t.frameY == 0;
        }

        public static void DustCircle(Vector2 center, int angle, int radius)
        {
            Main.dust[Dust.NewDust(center.Add(radius).RotatedBy(Math.PI * angle / 50), 1, 1, DustID.Fire, 0, 0, 128)].noGravity = true;
        }

        public static void DustRect(Rectangle rect)
        {
            Main.dust[Dust.NewDust(new Vector2(rect.X, rect.Y), 1, 1, DustID.Fire, 0, 0, 128)].noGravity = true;
            Main.dust[Dust.NewDust(new Vector2(rect.X + rect.Width, rect.Y), 1, 1, DustID.Fire, 0, 0, 128)].noGravity = true;
            Main.dust[Dust.NewDust(new Vector2(rect.X, rect.Y + rect.Height), 1, 1, DustID.Fire, 0, 0, 128)].noGravity = true;
            Main.dust[Dust.NewDust(new Vector2(rect.X + rect.Width, rect.Y + rect.Height), 1, 1, DustID.Fire, 0, 0, 128)].noGravity = true;
        }

        public static void CustomKnockback(NPC npc, float power, float x, float y)
        {
            if (npc.knockBackResist > 0)
            {
                float realPower = power * npc.knockBackResist;
                if (realPower > 8) realPower = 8 + (realPower - 8) * 0.9F;
                if (realPower > 10) realPower = 10 + (realPower - 10) * 0.8F;
                if (realPower > 12) realPower = 12 + (realPower - 12) * 0.7F;
                if (realPower > 14) realPower = 14 + (realPower - 14) * 0.6F;
                if (realPower > 16) realPower = 16;
                npc.velocity.Y = realPower * (npc.noGravity ? 0.5F : 0.75F) * npc.knockBackResist * y;
                npc.velocity.X = realPower * npc.knockBackResist * x;
            }
        }

        public static void RegenEffect(Entity en, int dust = 45, int sound = 25)
        {
            if (sound != -1) Main.PlaySound(sound);
            for (int i = 0; i < 7; ++i)
            {
                int d = Dust.NewDust(en.position, en.width, en.height, dust, 0, 0, 255, default(Color), Main.rand.Next(20, 26) / 10F);
                Main.dust[d].noLight = true;
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity /= 2;
            }
        }

        public static Point Rotate(Point p, float sin, float cos)
        {
            return new Point((int)(p.X * cos - p.Y * sin), (int)(p.X * sin + p.Y * cos));
        }

        public static Point[] Rect2Poly(Rectangle rect, Point origin = default(Point), float rot = 0)
        {
            Point[] ret = new Point[] { new Point(rect.X, rect.Y), new Point(rect.X + rect.Width, rect.Y), new Point(rect.X, rect.Y + rect.Height), new Point(rect.X + rect.Width, rect.Y + rect.Height) };
            if (rot != 0)
            {
                float s = (float)Math.Sin(rot);
                float c = (float)Math.Cos(rot);
                for (int i = 0; i < 4; ++i) ret[i] = Add(Rotate(Add(ret[i], origin, -1), s, c), origin, 1);
            }
            return ret;
        }

        public static Point Add(Point p1, Point p2, int mult)
        {
            return new Point(p1.X + p2.X * mult, p1.Y + p2.Y * mult);
        }

        public static bool PolygonIntersection(Point[] a, Point[] b)
        {
            foreach (var polygon in new[] { a, b })
                for (int i1 = 0; i1 < polygon.Length; ++i1)
                {
                    int i2 = (i1 + 1) % polygon.Length;
                    Point p1 = polygon[i1];
                    Point p2 = polygon[i2];
                    Point normal = new Point(p2.Y - p1.Y, p1.X - p2.X);
                    int? minA = null, maxA = null;
                    foreach (var p in a)
                    {
                        int projected = normal.X * p.X + normal.Y * p.Y;
                        if (minA == null || projected < minA) minA = projected;
                        if (maxA == null || projected > maxA) maxA = projected;
                    }
                    int? minB = null, maxB = null;
                    foreach (var p in b)
                    {
                        int projected = normal.X * p.X + normal.Y * p.Y;
                        if (minB == null || projected < minB) minB = projected;
                        if (maxB == null || projected > maxB) maxB = projected;
                    }
                    if (maxA < minB || maxB < minA) return false;
                }
            return true;
        }

        public static Vector2 Add(this Vector2 v, float x = 0, float y = 0)
        {
            return new Vector2(v.X + x, v.Y + y);
        }
    }
}