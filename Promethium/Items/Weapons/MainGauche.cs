using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Promethium.Items.Weapons
{
    public class MainGauche : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 11;
            item.name = "Main Gauche";
            item.width = 38;
            item.height = 38;
            item.toolTip = "'En garde!'";
            item.melee = true;
            item.useStyle = 3;
            item.useTime = 12;
            item.useAnimation = 12;
            item.autoReuse = false;
            item.knockBack = 6;
            item.value = Item.buyPrice(0, 2);
            item.rare = 2;
            item.UseSound = SoundID.Item1;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
        {
            OnHitPvp(player, null, damage, crit);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (item.ownTime == 0)
            {
                item.ownTime = 30;
                player.velocity /= 2;
                player.velocity.X -= player.direction * 8;
                player.velocity.Y -= 6;
                player.AddBuff(BuffID.Swiftness, 90, false);
            }
        }
    }
}