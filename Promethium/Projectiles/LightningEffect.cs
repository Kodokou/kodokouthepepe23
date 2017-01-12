using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Promethium.Projectiles
{
    class LightningEffect : ModProjectile
    {
        private static readonly Rectangle t1 = new Rectangle(0, 0, 64, 128);
        private static readonly Rectangle t2 = new Rectangle(63, 0, 1, 128);

        private struct Line
        {
            public Vector2 start, end;
            public Line(Vector2 v1, Vector2 v2) { start = v1; end = v2; }
        }

        private List<Line> lines = new List<Line>();

        public override void SetDefaults()
        {
            projectile.timeLeft = 30;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override bool CanDamage() { return false; }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                CreateBolt(new Vector2(projectile.ai[0], projectile.ai[1]), projectile.position);
            }
            foreach (Line l in lines) Lighting.AddLight(l.end, 0.9F, 0.9F, 1);
        }

        private void CreateBolt(Vector2 source, Vector2 dest)
        {
            Vector2 tangent = dest - source;
            Vector2 normal = Vector2.Normalize(new Vector2(tangent.Y, -tangent.X));
            float length = tangent.Length();

            var positions = new List<float>();
            positions.Add(0);

            for (int i = 0; i < length / 4; i++) positions.Add((float)Main.rand.NextDouble());

            positions.Sort();

            const float Sway = 160;
            const float Jaggedness = 1 / Sway;

            Vector2 prevPoint = source;
            float prevDisplacement = 0;
            for (int i = 1; i < positions.Count; ++i)
            {
                float pos = positions[i];
                float scale = (length * Jaggedness) * (pos - positions[i - 1]);
                float envelope = pos > 0.95F ? 20 * (1 - pos) : 1;

                float displacement = (float)Main.rand.NextDouble() * Sway * 2 - Sway;
                displacement -= (displacement - prevDisplacement) * (1 - scale);
                displacement *= envelope;

                Vector2 point = source + pos * tangent + displacement * normal;
                lines.Add(new Line(prevPoint, point));
                prevPoint = point;
                prevDisplacement = displacement;
            }
            lines.Add(new Line(prevPoint, dest));
        }

        public override bool PreDraw(SpriteBatch sb, Color tint)
        {
            Texture2D tex = ModLoader.GetTexture("Promethium/Projectiles/LightningEffect");
            const float thickScale = 0.03F;
            Vector2 capOrigin = new Vector2(tex.Width, tex.Height / 2F);
            Vector2 middleOrigin = new Vector2(0, tex.Height / 2F);
            for (int i = 0; i < lines.Count; ++i)
            {
                Line l = lines[i];
                Vector2 tangent = l.end - l.start;
                float theta = (float)Math.Atan2(tangent.Y, tangent.X);
                Vector2 middleScale = new Vector2(tangent.Length(), thickScale);
                float alpha = projectile.timeLeft / 30F - 0.3F + 0.3F * i / lines.Count;
                Color c = tint * (alpha < 0 ? 0 : alpha);
                sb.Draw(tex, l.start - Main.screenPosition, t2, c, theta, middleOrigin, middleScale, SpriteEffects.None, 0);
                sb.Draw(tex, l.start - Main.screenPosition, t1, c, theta, capOrigin, thickScale, SpriteEffects.None, 0);
                sb.Draw(tex, l.end - Main.screenPosition, t1, c, theta + MathHelper.Pi, capOrigin, thickScale, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
