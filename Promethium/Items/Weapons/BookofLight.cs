using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Promethium.Items.Weapons
{
    class BookofLight : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 14;
            item.name = "Book of Light";
            item.width = 28;
            item.height = 30;
            item.useStyle = 5;
            item.useAnimation = 30;
            item.useTime = 30;
            item.magic = true;
            item.value = Item.buyPrice(0, 2);
            item.mana = 20;
            item.rare = 2;
            item.noMelee = true;
            item.crit = 0;
            item.toolTip = "Shoots a stunning burst of photons";
            item.UseSound = SoundID.Item8;
        }

        public override bool UseItem(Player plr)
        {
            Vector2 pos = plr.Center;
            for (int i = 0; i < 100; ++i)
            {
                Vector2 v = Main.rand.NextVector2Circular(30, 30);
                Main.dust[Dust.NewDust(pos, 4, 4, DustID.AncientLight, v.X, v.Y, 0, default(Color), 2)].noGravity = true;
            }
            for (int i = 0; i < Main.maxNPCs; ++i)
            {
                Vector2 v = Main.rand.NextVector2Circular(30, 30);
                Main.dust[Dust.NewDust(pos, 4, 4, DustID.AncientLight, v.X, v.Y, 0, default(Color), 2)].noGravity = true;
                NPC n = Main.npc[i];
                if (n.CanBeChasedBy(plr) && Collision.CanHitLine(pos, 1, 1, n.position, n.width, n.height))
                {
                    Vector2 diff = pos - n.Center;
                    if (diff.LengthSquared() <= 180 * 180)
                    {
                        Utils.NetStrikeNPC(n, plr.GetWeaponDamage(item), plr.GetWeaponKnockback(item, item.knockBack), -Math.Sign(diff.X));
                        if (!n.boss && Main.rand.Next(4) != 0) n.AddBuff(mod.BuffType("Stun"), 150);
                    }
                }
            }
            return true;
        }
    }
}
