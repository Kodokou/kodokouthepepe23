using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Promethium.Items.Weapons
{
    class Necronomicon : ModItem
    {
        private int rightTime;
        public int necroCost;
        public string summon;

        public override void SetDefaults()
        {
            item.name = "Necronomicon";
            item.summon = true;
            item.noMelee = true;
            item.damage = 50;
            item.mana = 10;
            item.useStyle = 4;
            item.width = 28;
            item.height = 30;
            item.UseSound = SoundID.Item44;
            item.rare = 4;
            item.useAnimation = 20;
            item.useTime = 20;
            item.knockBack = 6;
            item.toolTip = "'The shadows beckon'";
            item.toolTip2 = "Allows collection of slain souls";
            item.value = Item.buyPrice(0, 15, 0, 0);
            summon = "";
            necroCost = 5;
        }

        public override bool AltFunctionUse(Player plr)
        {
            if (rightTime == 0)
            {
                // TODO: Remove pathfinding debug function
                try
                {
                    PathFinder pf = new PathFinder() { Debug = true };
                    pf.FindPath(plr, (Main.MouseScreen + Main.screenPosition).ToTileCoordinates(), 17);
                }
                catch (System.Exception ex) { ErrorLogger.Log(ex.ToString()); }
                /* TODO: Restore after debug ends
                if (summon == "")
                {
                    summon = "Warrior";
                    necroCost = 15;
                    item.mana = 20;
                }
                else if (summon == "Warrior")
                {
                    summon = "Mage";
                    necroCost = 30;
                    item.mana = 30;
                }
                else
                {
                    summon = "";
                    necroCost = 5;
                    item.mana = 10;
                }
                CombatText.NewText(new Rectangle((int)plr.position.X, (int)plr.position.Y, plr.width, plr.height), Color.LightGray, "Minion: Skeleton " + summon, false, false);
                */            
                rightTime = 30;
            }
            return false;
        }

        public override bool UseItem(Player plr)
        {
            plr.GetModPlayer<CCMPlayer>(mod).statNecro -= necroCost;
            if (Main.myPlayer == plr.whoAmI)
                Projectile.NewProjectile(plr.MountedCenter, plr.velocity / 2, mod.ProjectileType("Skeleton" + summon), plr.GetWeaponDamage(item), plr.GetWeaponKnockback(item, item.knockBack), plr.whoAmI);
            return true;
        }

        public override bool CanUseItem(Player plr)
        {
            return plr.GetModPlayer<CCMPlayer>(mod).statNecro >= necroCost;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; ++i)
                if (tooltips[i].Name == "UseMana")
                {
                    tooltips.Insert(i + 1, new TooltipLine(mod, "UseNecro", "Uses " + necroCost + " soul power"));
                    break;
                }
        }

        public override void UpdateInventory(Player player)
        {
            if (rightTime > 0) --rightTime;
        }
    }
}
