using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Promethium.Items.Weapons
{
    public class ManaBuckler : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mana Buckler");
            Tooltip.SetDefault("Lets your mana become your shield");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 34;
            item.useStyle = 5;
            item.useAnimation = 22;
            item.useTime = 22;
            item.value = Item.buyPrice(0, 2);
            item.mana = 10;
            item.rare = 3;
            item.UseSound = SoundID.Item8;
        }

        public override bool UseItem(Player plr)
        {
            plr.GetModPlayer<CCMPlayer>(mod).manaBucklerLeft = 5;
            plr.AddBuff(mod.BuffType<Buffs.ManaBuckler>(), 7200, false);
            return true;
        }
    }
}

