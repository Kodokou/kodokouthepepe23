using Terraria;
using Terraria.ID;

namespace Promethium.Projectiles.Items
{
    class GauntletofEmbers : AnimItem
    {
        public override void SetDefaults(ref int frames, ref int animSpeed)
        {
            projectile.name = "Gauntlet of Embers";
            frames = 1;
            animSpeed = 666;
            projectile.width = 34;
            projectile.height = 34;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "Promethium/Items/Weapons/GauntletofEmbers";
            return true;
        }

        public override void CustomAI()
        {
            UpdateRotation();
            Player plr = Main.player[projectile.owner];
            if (projectile.ai[0] < 60 && ++projectile.ai[0] % 30 == 0)
            {
                if (!plr.CheckMana(15, true)) projectile.ai[0] -= 30;
                plr.manaRegenDelay = (int)plr.maxRegenDelay;
                for (int i = 0; i < 20; ++i)
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, ProjectileID.Fireball);
                Main.PlaySound(SoundID.Item13, projectile.Center);
            }
        }

        public override void Kill(int timeLeft)
        {
            // TODO: CHANGE THIS TO OTHER BALLZ
            for (int i = (int)projectile.ai[0] / 30; i >= 0; --i)
                ShootProjectile("FireballSmall", 10 + i * 2, SoundID.Item20);
        }
    }
}
