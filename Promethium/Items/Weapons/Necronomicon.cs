using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using System.Collections.Generic;

namespace Promethium.Items.Weapons
{
    class Necronomicon : ModItem
    {
        public int necroCost = 5;

        public override void SetDefaults()
        {
            item.name = "Necronomicon";
            item.summon = true;
            item.noMelee = true;
            item.damage = 30;
            item.mana = 10;
            // TODO: Add more properties
        }

        public override bool AltFunctionUse(Player player)
        {
            // TODO: Minion type selection (window?)
            // Update item.shoot, item.mana and statNecro cost
            return false;
        }

        public override bool UseItem(Player plr)
        {
            plr.GetModPlayer<CCMPlayer>(mod).statNecro -= necroCost;
            return false;
        }

        public override bool CanUseItem(Player plr)
        {
            return plr.GetModPlayer<CCMPlayer>(mod).statNecro >= necroCost;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // TODO: Add soul cost tooltip right after mana cost tooltip
        }
    }
}
