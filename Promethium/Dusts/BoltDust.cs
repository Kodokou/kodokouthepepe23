using Terraria.ModLoader;
using Terraria;

namespace Promethium.Dusts
{
    class BoltDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
        }

        public override bool Update(Dust dust)
        {
            dust.alpha += 4;
            float light = dust.alpha / 240F - 0.4F;
            Lighting.AddLight(dust.position, light, light, light * 1.1F);
            if (dust.alpha > 250) dust.active = false;
            return false;
        }
    }
}
