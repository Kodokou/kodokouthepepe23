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
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.timeLeft = 80;
            projectile.alpha = 255;
            projectile.scale = 0.01F;
            projectile.penetrate = 1;
            projectile.extraUpdates = 20;
            Main.projFrames[projectile.type] = 1;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                if (Main.raining) projectile.damage = projectile.damage * 3 / 2;
                projectile.ai[0] = projectile.position.X;
                projectile.ai[1] = projectile.position.Y;
                projectile.netUpdate = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            Vector2 origin = new Vector2(projectile.ai[0], projectile.ai[1]);
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
            Vector2 tangent = end - start;
            float theta = (float)System.Math.Atan2(tangent.Y, tangent.X);
            int dust = Dust.NewDust(start, 1, 1, 269, tangent.X / 2, tangent.Y / 2, 0, Color.White, 1.2F);
            Dust d = Main.dust[dust];
            d.rotation = theta;
            d.noGravity = true;
        }
    }
}