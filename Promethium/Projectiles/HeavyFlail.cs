using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Promethium.Projectiles
{
    public class HeavyFlail : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.name = "HeavyFlail";
            projectile.width = 40;
            projectile.height = 40;
            projectile.aiStyle = 15;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = (int)(damage * GetDamageMult());
            knockback *= GetDamageMult();
        }

        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit)
        {
            damage = (int)(damage * GetDamageMult());
        }

        private float GetDamageMult()
        {
            return 1 + projectile.velocity.LengthSquared() / 250;
        }

        public override bool PreDraw(SpriteBatch sb, Color lightColor)
        {
            Texture2D chainTex = ModLoader.GetTexture("Promethium/Projectiles/HeavyFlailChain");
            Vector2 projPos = projectile.Center;
            Vector2 plrPos = Main.player[projectile.owner].MountedCenter;
            Vector2 origin = new Vector2(chainTex.Width / 2F, chainTex.Height / 2F);
            float texH = chainTex.Height;
            Vector2 diff = plrPos - projPos;
            float rotation = diff.ToRotation() - (float)Math.PI / 2;
            bool draw = true;
            if (float.IsNaN(projPos.X) && float.IsNaN(projPos.Y)) draw = false;
            if (float.IsNaN(diff.X) && float.IsNaN(diff.Y)) draw = false;
            while (draw)
            {
                if (diff.Length() < texH + 1) draw = false;
                else
                {
                    projPos += Vector2.Normalize(diff) * texH;
                    diff = plrPos - projPos;
                    Color lighting = Lighting.GetColor((int)(projPos.X / 16), (int)(projPos.Y / 16));
                    lighting = projectile.GetAlpha(lighting);
                    sb.Draw(chainTex, projPos - Main.screenPosition, null, lighting, rotation, origin, 1, SpriteEffects.None, 0);
                }
            }
            return true;
        }
    }
}
