﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman
{
    enum ComponentType
    {
        Bomb = 1,
        Collision,
        InputContext,
        Message,
        PlayerInfo,
        PowerUp,
        ScreenPosition,
        Spreadable,
        Sprite,
        TilePosition,
        TimedEffect
    }

    abstract class Component
    {
        // ID that ties components to an Entity
        public int entityID { get; private set; }

        // Life of component
        public bool live = true;

        // Component type ID
        public abstract ComponentType type { get; }

        /// <summary>
        /// Set the entity ID
        /// </summary>
        public void SetOwnerEntity(int ID)
        {
            entityID = ID;
        }
    }
}