using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework;
using System;

namespace Promethium.Projectiles.Items
{
    public abstract class AnimItem : ModProjectile
    {
        public int frames, animSpeed;
        public float rotShift = MathHelper.PiOver4;
        public bool frontDraw = false;

        public sealed override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            SetDefaults(ref frames, ref animSpeed);
            Main.projFrames[projectile.type] = frames;
        }

        public override bool CanDamage() { return false; }

        public abstract void SetDefaults(ref int frames, ref int animSpeed);

        public virtual void Action() { }

        public virtual void CustomAI() { }

        public void ShootProjectile(int id, float speed, Terraria.Audio.LegacySoundStyle sound = null)
        {
            Vector2 v = projectile.Center;
            if (sound != null) Main.PlaySound(sound, v);
            if (Main.myPlayer == projectile.owner)
                Projectile.NewProjectile(v, Vector2.Normalize(projectile.velocity) * speed, id, projectile.damage, projectile.knockBack, projectile.owner);
        }

        public virtual void Animate()
        {
            if (++projectile.frameCounter >= animSpeed)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= frames) projectile.frame = 0;
            }
        }

        public void UpdateRotation()
        {
            Player plr = Main.player[projectile.owner];
            Vector2 rotRelPos = plr.RotatedRelativePoint(plr.MountedCenter);
            if (Main.myPlayer == projectile.owner)
            {
                if (plr.channel)
                {
                    float plrDist = plr.inventory[plr.selectedItem].shootSpeed * projectile.scale;
                    float posX = Main.mouseX + Main.screenPosition.X - rotRelPos.X;
                    float posY = Main.screenPosition.Y - rotRelPos.Y;
                    posY += plr.gravDir == -1 ? Main.screenHeight - Main.mouseY : Main.mouseY;
                    float mult = plrDist / (float)Math.Sqrt(posX * posX + posY * posY);
                    posX *= mult;
                    posY *= mult;
                    if (posX != projectile.velocity.X || posY != projectile.velocity.Y) projectile.netUpdate = true;
                    projectile.velocity.X = posX;
                    projectile.velocity.Y = posY;
                }
            }
            projectile.position.X = rotRelPos.X - projectile.width / 2F;
            projectile.position.Y = rotRelPos.Y - projectile.height / 2F;
            if (projectile.velocity.X > 0) plr.ChangeDir(1);
            else if (projectile.velocity.X < 0) plr.ChangeDir(-1);
            projectile.spriteDirection = projectile.direction = plr.direction;
            Vector2 rot = projectile.velocity * plr.direction;
            plr.itemRotation = rot.ToRotation();
        }

        public sealed override void Kill(int timeLeft) { }

        public sealed override void AI()
        {
            Animate();
            Player plr = Main.player[projectile.owner];
            Vector2 rotRelPos = plr.RotatedRelativePoint(plr.MountedCenter);
            plr.heldProj = projectile.whoAmI;
            plr.itemTime = 2;
            plr.itemAnimation = 2;
            projectile.position.X = rotRelPos.X - projectile.width / 2F;
            projectile.position.Y = rotRelPos.Y - projectile.height / 2F;
            if (projectile.localAI[1] > 0)
            {
                if (++projectile.localAI[1] > 15) projectile.Kill();
            }
            else if (Main.myPlayer == projectile.owner && !plr.channel)
            {
                projectile.localAI[1] = 1;
                Action();
            }
            else CustomAI();
            projectile.rotation = plr.itemRotation + rotShift * plr.direction;
        }
    }
}