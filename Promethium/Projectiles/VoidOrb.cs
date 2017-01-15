using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace Promethium.Projectiles
{
    class VoidOrb : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.name = "Void Orb";
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.timeLeft = 96;
            projectile.ignoreWater = true;
            projectile.minion = true;
            projectile.extraUpdates = 1;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "Promethium/Items/Weapons/VoidOrb";
            return true;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0 && Main.myPlayer == projectile.owner)
            {
                projectile.ai[0] = Main.mouseX + Main.screenPosition.X;
                projectile.ai[1] = Main.mouseY + Main.screenPosition.Y;
                projectile.netUpdate = true;
            }
            else if (projectile.DistanceSQ(new Vector2(projectile.ai[0], projectile.ai[1])) < 6) projectile.Kill();
        }

        public override void Kill(int timeLeft)
        {
            if (Main.myPlayer == projectile.owner)
            {
                int vortexId = mod.ProjectileType("Vortex");
                for (int i = 0; i < Main.maxNPCs; ++i)
                {
                    Projectile p = Main.projectile[i];
                    if (p.owner == projectile.owner && p.type == vortexId && p.timeLeft > 30) p.timeLeft = 30;
                }
                Projectile.NewProjectile(projectile.Center + projectile.velocity, Vector2.Zero, vortexId, projectile.damage, projectile.knockBack, projectile.owner);
            }
        }
    }
}
