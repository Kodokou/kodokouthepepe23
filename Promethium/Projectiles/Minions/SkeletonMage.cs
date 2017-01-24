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
            projectile.penetrate = -1;
            item = Main.itemTexture[ItemID.RubyStaff];
        }

        public override void AI()
        {
            // TODO: Shooting AI, maybe modified Pygmy AI?
            base.AI();
        }

        public override bool MinionContactDamage() { return false; }
    }
}
