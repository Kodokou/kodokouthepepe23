using Terraria;
using System;

namespace Promethium.AI.Entities
{
    class WrapNPC : AIUser
    {
        protected readonly new NPC entity;

        public WrapNPC(NPC obj) : base(obj)
        {
            entity = obj;
            data = BitConverter.GetBytes(obj.ai[0]);
        }

        public override Player GetPlayer() { return Main.player[entity.FindClosestPlayer()]; }

        public override bool IsFriendly() { return entity.friendly; }

        public override void SetTileCollide(bool value) { entity.noTileCollide = !value; }

        public override void CollisionStepUp()
        {
            Collision.StepUp(ref entity.position, ref entity.velocity, entity.width, entity.height, ref entity.stepSpeed, ref entity.gfxOffY, 1, false, 0);
        }

        public override float GetFloatData() { return entity.ai[1]; }

        public override void SetFloatData(float value) { entity.ai[1] = value; }

        public override int GetTimeLeft() { return entity.timeLeft; }

        public override void SetUpdate() { entity.netUpdate = true; }

        public override void SetRotation(float value) { entity.rotation = value; }

        public override int GetMinionPos() { return -1; }

        public override void SaveData() { entity.ai[0] = BitConverter.ToSingle(data, 0); }
    }
}