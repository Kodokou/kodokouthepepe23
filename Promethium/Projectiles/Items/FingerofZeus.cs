using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Promethium.Projectiles.Items
{
    class FingerofZeus : AnimItem
    {
        public override void SetDefaults(ref int frames, ref int animSpeed)
        {
            projectile.name = "Finger of Zeus";
            projectile.width = 40;
            projectile.height = 40;
            frames = 3;
            animSpeed = 10;
        }

        public override bool Autoload(ref string name, ref string texture)
        {
            texture = "Promethium/Items/Weapons/FingerofZeus";
            return true;
        }

        public override void CustomAI()
        {
            if (Main.myPlayer == projectile.owner)
            {
                Player plr = Main.player[projectile.owner];
                if (projectile.localAI[0]++ == 0 && plr.CheckMana(5, true))
                {
                    plr.manaRegenDelay = (int)plr.maxRegenDelay;
                    UpdateRotation();
                    ShootProjectile("LightningStrike", 6, SoundID.Item20);
                }
                else if (projectile.localAI[0] == 10) projectile.localAI[0] = 0;
            }
        }
    }
}