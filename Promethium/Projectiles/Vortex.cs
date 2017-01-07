using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Promethium.Projectiles
{
	public class Vortex : ModProjectile
	{
		List<NPC> targets = new List<NPC> ();
		List<Vector2> vels = new List<Vector2> ();
		int i = 0;

		public override void SetDefaults ()
		{
			projectile.damage = 50;
			projectile.rotation = 0;
			projectile.alpha = 255;
			//TODO Get some stuff here
		}

		public override bool PreAI ()
		{
			//So, this vortex should suck in anything close to it
			//So we're getting the NPCs that have to be sucked
			//I'm not sure how to update it dynamically in another way then this, which seems the best

			foreach(NPC n in Main.npc)
				if(n.DistanceSQ(projectile.Center) <= 6400 && n.active) //(16 * 5)^2, since 16 is a tile's width
				{
					targets.Add (n);
					vels.Add (n.velocity);		
					n.velocity = Vector2.Zero;
				}
			return true;

		}

		public override void AI ()
		{
			if (projectile.alpha > 0)
				projectile.alpha -= 5;
			for (int i = 0; i < targets.Count; i++) {
				Vector2 velToAspire = (projectile.position - targets [i].position);
				velToAspire.Normalize ();
				velToAspire *= 5f;
				targets [i].velocity = Lerp (vels [i], velToAspire, 1);
			}
		}

		public override void PostAI ()
		{
			targets.Clear ();
			vels.Clear ();
		}

		//So I figure that clientside this would work fine, but serverside it wouldn't
		//which is why these hooks exist
		/*also if you're wondering why I don't comment like this it's because I find it ugly*/

		public override void SendExtraAI (System.IO.BinaryWriter writer)
		{
			foreach (NPC n in targets)
				writer.Write (n.whoAmI);
		}

		public override void ReceiveExtraAI (System.IO.BinaryReader reader)
		{
			targets.Add (Main.npc [reader.ReadInt32()]);
		}

		public static Vector2 Lerp(Vector2 origin, Vector2 target, float step = 0.1f)
		{
			Vector2 ret = origin, stepVel = target - origin;
			stepVel.Normalize ();
			stepVel *= step;
			ret += stepVel;	
			return ret == target ? Vector2.Zero : ret;
		}
	}
}

