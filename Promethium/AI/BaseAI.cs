﻿using Terraria;

namespace Promethium.AI
{
    public abstract class BaseAI
    {
        /// <summary>Executes this AI logic on the specified Entity</summary>
        /// <returns>True to continue using this AI, false otherwise</returns>
        public abstract bool AI(AIUser aiu);

        /// <summary>Called to check prerequisites for this AI</summary>
        public abstract bool CanStart(AIUser aiu);
    }
}
