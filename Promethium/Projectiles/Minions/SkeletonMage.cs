using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using System;

namespace Promethium.Projectiles.Minions
{
    class SkeletonMage : SkeletonWarrior
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            projectile.name = "Skeleton Mage";
            item = Main.itemTexture[ItemID.RubyStaff];
            necroDrain = 0.001F;
            //attackDist = 300;
        }

        public override bool Attack(NPC target, float dist)
        {
            if (Collision.CanHitLine(projectile.Center, 8, 8, target.Center, target.width, target.width))
            {
                projectile.velocity /= 10;
                Vector2 v = Vector2.Normalize(target.Center - projectile.Center) * 9;
                if (Main.myPlayer == projectile.owner) Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, v.X, v.Y, ProjectileID.RubyBolt, projectile.damage, projectile.knockBack, projectile.owner);
                return false;
            }
            return true;
        }

        public override bool MinionContactDamage() { return false; }
    }
}