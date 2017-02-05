using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Promethium.Projectiles
{
    class ChronoPrism : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.name = "Chrono Prism";
            projectile.alpha = 255;
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.timeLeft = 1800;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.minion = true;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = projectile.damage;
                projectile.damage = 1;
                int count = 0, shortestTimeLeft = projectile.timeLeft, shortestID = -1;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.type == projectile.type && p.owner == projectile.owner)
                    {
                        if (p.timeLeft < shortestTimeLeft)
                        {
                            shortestTimeLeft = p.timeLeft;
                            shortestID = i;
                        }
                        ++count;
                    }
                }
                if (count > 2) Main.projectile[shortestID].Kill();
            }
            if (projectile.alpha > 10) projectile.alpha -= 10;
            projectile.velocity *= 0.9F;
            projectile.rotation += 0.01F;
            if (Main.rand.Next(2) == 0)
                Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, projectile.velocity.X / 2, projectile.velocity.Y / 2, 192)].noGravity = true;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 7; ++i)
                Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 229, projectile.velocity.X / 2, projectile.velocity.Y / 2, 192)].noGravity = true;
        }

        public override bool PreDraw(SpriteBatch sb, Color light)
        {
            Vector2 pos = projectile.Center - Main.screenPosition;
            Texture2D tex = mod.GetTexture("Projectiles/ChronoPrism_1");
            float rot = (projectile.timeLeft % 360) * MathHelper.TwoPi / 180;
            sb.Draw(tex, pos, null, light, rot, tex.Size() / 2, 1, SpriteEffects.None, 0);
            tex = mod.GetTexture("Projectiles/ChronoPrism_2");
            rot = (projectile.timeLeft % 270) * MathHelper.TwoPi / -120;
            sb.Draw(tex, pos, null, light, rot, tex.Size() / 2, 1, SpriteEffects.None, 0);
            return true;
        }
    }
}
