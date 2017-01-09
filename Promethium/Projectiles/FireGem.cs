using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace Promethium.Projectiles
{
	public class FireGem : ModProjectile
	{
		private const int MAX_DIST_SQ = 16 * 5 * 16 * 5;

		public override void SetDefaults()
		{
			projectile.name = "Fire Gem";
			projectile.width = 20;
			projectile.height = 20;
			projectile.penetrate = -1;
			projectile.friendly = true;
			projectile.timeLeft = 60 * 30;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			Main.projFrames[projectile.type] = 1;
		}

		public override bool Autoload(ref string name, ref string texture)
		{
			texture = "Promethium/Items/Weapons/FireGem";
			return true;
		}

		public override void AI()
        {
            if (projectile.localAI[0] == 0)
            {
                projectile.localAI[0] = 1;
                int count = 0, shortestTimeLeft = 9999, shortestID = -1;
                for (int i = 0; i < 200; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.type == mod.ProjectileType("FireGem") && p.owner == Main.myPlayer)
                    {
                        if (p.timeLeft < shortestTimeLeft)
                        {
                            shortestTimeLeft = p.timeLeft;
                            shortestID = i;
                        }
                        count++;
                    }
                }
                if (count > 2) Main.projectile[shortestID].Kill();
            }
			Vector2 tilePos = projectile.Center / 16;
            if (tilePos.X >= 0 && tilePos.X < Main.maxTilesX && tilePos.Y >= 0 && tilePos.Y < Main.maxTilesY)
            {
                Tile t = Main.tile[(int)tilePos.X, (int)tilePos.Y];
                if (t != null && t.nactive() && (Main.tileSolid[t.type] || (Main.tileSolidTop[t.type] && t.frameY == 0)))
                {
                    projectile.velocity = Vector2.Zero;
                    if (projectile.ai[0] == 0)
                    {
                        projectile.ai[0] = 1;
                        projectile.netUpdate = true; 
                    }
                }
                else projectile.velocity.Y += 2;
            }
            if (projectile.ai[0] == 1)
				for (int i = 0; i < 200; ++i)
				{
					NPC n = Main.npc[i];
                    if (n.CanBeChasedBy(projectile))
                    {
                        Vector2 diff = projectile.Center - n.Center;
                        float distSq = diff.LengthSquared();
                        if (distSq <= MAX_DIST_SQ)
                        {
                            projectile.netUpdate = true;
                            projectile.ai[0] = 2;
                        }
                    }
				}
			else if (projectile.ai[0] > 1)
				if (++projectile.ai[0] > 10)
					projectile.Kill();
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 200; ++i)
			{
				NPC n = Main.npc[i];
				if (n.CanBeChasedBy(projectile))
				{
					Vector2 diff = projectile.Center - n.Center;
					float distSq = diff.LengthSquared();
					if (distSq <= MAX_DIST_SQ)
					{
						n.StrikeNPC(projectile.damage, projectile.knockBack, (int)Math.Sign(diff.X));
					}
				}
			}
			/*for (int i = -40; i < 40; i += 4)
			{
				for (int j = -40; i < 40; i += 4)
				{
					Vector2 potentPos = new Vector2(projectile.position.X + i, projectile.position.Y + j);
					if (potentPos.LengthSquared() <= MAX_DIST_SQ)
						Dust.NewDust(potentPos, 4, 4, DustID.Fire);
				}
			}*/
			float count = Main.rand.Next(27, 33);
			for (int i = 0; i < count; i++)
			{
				Vector2 vel = Main.rand.NextVector2Circular(5, 5);
				Dust.NewDust(projectile.Center, 4, 4, DustID.Smoke, vel.X, vel.Y);
			}
		}
	}
}

