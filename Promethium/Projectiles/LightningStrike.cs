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
            CreateBolt(origin, projectile.Center);
            base.Kill(timeLeft);
        }

        private void CreateBolt(Vector2 source, Vector2 dest)
        {
            Vector2 tangent = dest - source;
            Vector2 normal = Vector2.Normalize(new Vector2(tangent.Y, -tangent.X));
            float length = tangent.Length();

            var positions = new System.Collections.Generic.List<float>();
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
                NewLineAt(prevPoint, point, 128 - 128 * i / positions.Count);
                prevPoint = point;
                prevDisplacement = displacement;
            }
            NewLineAt(prevPoint, dest, 0);
        }

        private void NewLineAt(Vector2 start, Vector2 end, int alpha)
        {
            Vector2 tangent = end - start;
            float theta = (float)System.Math.Atan2(tangent.Y, tangent.X);
            int dust = Dust.NewDust((start + end) / 2, 1, 1, mod.DustType("BoltDust"), 0, 0, alpha);
            Main.dust[dust].rotation = theta;
        }
    }
}