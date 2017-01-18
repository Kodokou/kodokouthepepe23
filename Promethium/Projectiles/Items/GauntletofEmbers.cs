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
            frontDraw = true;
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
            if (projectile.ai[0] < 80 && ++projectile.ai[0] % 40 == 0)
            {
                if (plr.CheckMana(15, true))
                {
                    projectile.damage *= 2;
                    Utils.RegenEffect(plr);
                }
                else projectile.ai[0] -= 40;
                plr.manaRegenDelay = (int)plr.maxRegenDelay;
            }
        }

        public override void Action()
        {
            string proj = "Fireball";
            if (projectile.ai[0] >= 80) proj += "Large";
            else if (projectile.ai[0] >= 40) proj += "Med";
            else proj += "Small"; 
            ShootProjectile(mod.ProjectileType(proj), 12, SoundID.Item20);
        }
    }
}
