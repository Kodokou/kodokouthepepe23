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
            item.useStyle = 3;
            item.useTime = 12;
            item.useAnimation = 12;
            item.autoReuse = false;
            item.knockBack = 6;
            item.value = Item.buyPrice(0, 2);
            item.rare = 6;
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
            if (player.itemAnimation > player.itemAnimationMax * 0.666f)
            {
                player.itemLocation.X = -1000f;
                player.itemLocation.Y = -1000f;
                player.itemRotation = -1.3f * (float) this.direction;
            }
            else
            {
                this.itemLocation.X = (float) ((double) this.position.X + (double) this.width * 0.5 + ((double) Main.itemTexture[sItem.type].Width * 0.5 - 4.0) * (double) this.direction);
                this.itemLocation.Y = this.position.Y + 24f + playerOffsetHitbox;
                float num = (float) ((double) this.itemAnimation / (double) this.itemAnimationMax * (double) Main.itemTexture[sItem.type].Width * (double) this.direction * (double) sItem.scale * 1.20000004768372) - (float) (10 * this.direction);
                if ((double) num > -4.0 && this.direction == -1)
                    num = -8f;
                if ((double) num < 4.0 && this.direction == 1)
                    num = 8f;
                this.itemLocation.X -= num;
                this.itemRotation = 0.8f * (float) this.direction;
            }
            if ((double) this.gravDir == -1.0)
            {
                this.itemRotation = -this.itemRotation;
                this.itemLocation.Y = (float) ((double) this.position.Y + (double) this.height + ((double) this.position.Y - (double) this.itemLocation.Y));
            }
        }
    }
}

