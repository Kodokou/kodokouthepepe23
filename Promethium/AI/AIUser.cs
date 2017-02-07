using Terraria;
using System;

namespace Promethium.AI
{
    public abstract class AIUser
    {
        public readonly Entity entity;
        protected byte[] data;

        public AIUser(Entity obj) { entity = obj; }

        public abstract Player GetPlayer();

        public abstract bool IsFriendly();

        public abstract void SetTileCollide(bool value);

        public abstract void CollisionStepUp();

        public abstract float GetFloatData();
        public abstract void SetFloatData(float value);

        public abstract int GetTimeLeft();

        public abstract void SetUpdate();

        public abstract void SetRotation(float value);

        public abstract int GetMinionPos();

        public abstract void SaveData();

        public short GetIntData() { return BitConverter.ToInt16(data, 1); }

        public void SetIntData(short value) { BitConverter.GetBytes(value).CopyTo(data, 1); }

        public byte GetAI() { return data[0]; }

        public void SetAI(byte index) { data[0] = index; SetUpdate(); }
    }
}
