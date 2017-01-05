using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;

namespace Promethium.Projectiles
{
    class LightningStrike : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.name = "Lightning Strike";
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.timeLeft = 16;
            projectile.alpha = 250;
            projectile.scale = 0.1F;
            projectile.aiStyle = 1;
            projectile.penetrate = 1;
            Main.projFrames[projectile.type] = 1;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = projectile.position.X;
                projectile.localAI[1] = projectile.position.Y;
            }
            base.AI();
        }

        public override void Kill(int timeLeft)
        {
            Vector2 origin = new Vector2(projectile.localAI[0], projectile.localAI[1]);
            Vector2 pos = projectile.Center;
            CreateBolt(origin, pos);
            base.Kill(timeLeft);
        }

        private void CreateBolt(Vector2 source, Vector2 dest)
        {
            Vector2 tangent = dest - source;
            Vector2 normal = Vector2.Normalize(new Vector2(tangent.Y, -tangent.X));
            float length = tangent.Length();

            var positions = new System.Collections.Generic.List<float>();
            positions.Add(0);

            for (int i = 0; i < length / 4; i++) positions.Add(Rand(0, 1));

            positions.Sort();

            const float Sway = 160;
            const float Jaggedness = 1 / Sway;

            Vector2 prevPoint = source;
            float prevDisplacement = 0;
            for (int i = 1; i < positions.Count; i++)
            {
                float pos = positions[i];

                // used to prevent sharp angles by ensuring very close positions also have small perpendicular variation.
                float scale = (length * Jaggedness) * (pos - positions[i - 1]);

                // defines an envelope. Points near the middle of the bolt can be further from the central line.
                float envelope = pos > 0.95f ? 20 * (1 - pos) : 1;

                float displacement = Rand(-Sway, Sway);
                displacement -= (displacement - prevDisplacement) * (1 - scale);
                displacement *= envelope;

                Vector2 point = source + pos * tangent + displacement * normal;
                NewLineAt(prevPoint, point);
                prevPoint = point;
                prevDisplacement = displacement;
            }
            NewLineAt(prevPoint, dest);
        }

        private static float Rand(float min, float max)
        {
            return (float)Main.rand.NextDouble() * (max - min) + min;
        }

        private void NewLineAt(Vector2 start, Vector2 end)
        {
            /*
                const float ImageThickness = 8;
                float thicknessScale = Thickness / ImageThickness;

                Vector2 capOrigin = new Vector2(t1.Width, t1.Height / 2f);
                Vector2 middleOrigin = new Vector2(0, t2.Height / 2f);
                Vector2 middleScale = new Vector2(tangent.Length(), thicknessScale);

                spriteBatch.Draw(t2, start, null, tint, theta, middleOrigin, middleScale, SpriteEffects.None, 0f);
            */
            Vector2 tangent = end - start;
            float theta = (float)System.Math.Atan2(tangent.Y, tangent.X);
            int dust = Dust.NewDust(end, 3, 3, 64);
            Dust d = Main.dust[dust];
            d.rotation = theta;
            d.noGravity = true;
        }
    }
}