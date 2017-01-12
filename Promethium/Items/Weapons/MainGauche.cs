using System;
using Terraria;
using Terraria.ModLoader;

namespace Promethium.Items.Weapons
{
    public class MainGauche : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 10;
            item.name = "Main Gauche";
            item.width = 38;
            item.height = 38;
            item.toolTip = "Sample Text";
            item.melee = true;
            //  item.useStyle = 3;
            item.useTime = 12;
            item.useAnimation = 12;
            item.autoReuse = false;
            item.knockBack = 6;
            item.value = Item.buyPrice(0, 2);
            item.rare = 2;
            item.UseSound = Terraria.ID.SoundID.Item1;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            player.velocity.X = -player.direction * 4;
            player.velocity.Y = -4;
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            player.velocity.X = -player.direction * 4;
            player.velocity.Y = -4;
        }

        public override void UseStyle(Player player)
        {
            bool flag2 = false;
            if (player.itemAnimation > player.itemAnimationMax * 0.666) //when itemanimation is through 2/3 of it's sicle
            {
                player.itemLocation.X = -1000f;
                player.itemLocation.Y = -1000f;
                player.itemRotation = -1.3f * (float) player.direction;
            }
            else
            {
                player.itemLocation.X = (float) (player.position.X + player.width * 0.5 + (Main.itemTexture[item.type].Width * 0.5 - 4.0) * player.direction);
                player.itemLocation.Y = player.position.Y + 24f + player.mount.PlayerOffsetHitbox;
                float num = (player.itemAnimation / player.itemAnimationMax * Main.itemTexture[item.type].Width * player.direction * item.scale * 1.2f - 10 * player.direction);
                if ((double) num > -4.0 && player.direction == -1)
                    num = -8f;
                if ((double) num < 4.0 && player.direction == 1)
                    num = 8f;
                player.itemLocation.X -= num;
                player.itemRotation = 0.8f * (float) player.direction;
            }
            if ((double) player.gravDir == -1.0)
            {
                player.itemRotation = -player.itemRotation;
                player.itemLocation.Y = (float) ( player.position.Y + player.height + (player.position.Y - player.itemLocation.Y));
            }
            if ((double) this.itemAnimation > (double) this.itemAnimationMax * 0.666)
            {
                flag2 = true;
            }
            else
            {
                if (this.direction == -1)
                    r.X -= (int) ((double) r.Width * 1.4 - (double) r.Width);
                r.Width = (int) ((double) r.Width * 1.4);
                r.Y += (int) ((double) r.Height * 0.6);
                r.Height = (int) ((double) r.Height * 0.6);
            }
        }
        }
    }
}

