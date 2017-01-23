using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;

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
            drawHeldProjInFrontOfHeldItemAndArms = true;
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
            if (projectile.frameCounter % 3 == 0)
            {
                float charge = projectile.ai[0] * 16 / 40;
                Vector2 v = Main.rand.NextVector2CircularEdge(charge, charge);
                Vector2 pv = Main.player[projectile.owner].velocity * 2 / 3;
                Dust d = Main.dust[Dust.NewDust(Main.player[projectile.owner].MountedCenter + projectile.velocity + v, 1, 1, 127, -v.X / 2 + pv.X, -v.Y / 2 + pv.Y, 96, default(Color), 1.25F)];
                d.noGravity = true;
                d.noLight = true;
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
