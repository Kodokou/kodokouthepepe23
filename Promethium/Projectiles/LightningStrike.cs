using Terraria.ModLoader;
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
            projectile.timeLeft = 100;
            projectile.penetrate = 1;
            projectile.extraUpdates = 20;
            projectile.hide = true;
            projectile.magic = true;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                if (Main.raining) projectile.damage = projectile.damage * 3 / 2;
                Vector2 v = Main.player[projectile.owner].Center + projectile.velocity * 7;
                projectile.ai[0] = v.X;
                projectile.ai[1] = v.Y;
                projectile.netUpdate = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            SpawnEffect(projectile.Center);
        }

        private void SpawnEffect(Vector2 endPos)
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                if (Main.myPlayer == projectile.owner)
                    Projectile.NewProjectile(endPos, Vector2.Zero, mod.ProjectileType("LightningEffect"), 0, 0, projectile.owner, projectile.ai[0], projectile.ai[1]);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Point p = projectile.Center.ToPoint();
            return targetHitbox.Intersects(new Rectangle(p.X - 12, p.Y - 12, 24, 24));
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            SpawnEffect(target.Center);
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            SpawnEffect(target.Center);
        }
    }
}