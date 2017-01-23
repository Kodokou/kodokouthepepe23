using Terraria.ModLoader;
using Terraria.ID;
using Terraria;

namespace Promethium.Projectiles.Minions
{
    class Skeleton : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.name = "Skeleton";
            // TODO: Rest of stuff
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                SpawnEffect();
            }
            // TODO: Targeting logic, maybe just use pirate aiStyle instead
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // TODO: Self-destruct
            SpawnEffect();
        }

        private void SpawnEffect()
        {
            // TODO: Minion spawned from bones effect
        }
    }
}
