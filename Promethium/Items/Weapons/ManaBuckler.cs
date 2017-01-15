using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Promethium.Items.Weapons
{
    public class ManaBuckler : ModItem 
    {
        public override void SetDefaults()
        {
            item.name = "Mana Buckler";
            item.width = 22;
            item.height = 34;
            item.useStyle = 5;
            item.useAnimation = 22;
            item.useTime = 22;
            item.value = Item.buyPrice(0, 2);
            item.mana = 10;
            item.rare = 3;
            item.toolTip = "Mana becomes shield";
            item.UseSound = SoundID.Item8;
        }

        public override bool UseItem(Player plr)
        {
            plr.GetModPlayer<CCMPlayer>(mod).manaBucklerLeft = 5;
            plr.AddBuff(mod.BuffType("ManaBuckler"), 7200);
            return true;
        }
    }
}

