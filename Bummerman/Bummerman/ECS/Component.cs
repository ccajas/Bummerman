using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman
{
    enum ComponentType
    {
        Collision = 1,
        InputContext,
        PlayerInfo,
        PowerUp,
        ScreenPosition,
        Sprite,
        TilePosition
    }

    abstract class Component
    {
        // ID that ties components to an Entity
        protected int entityID = -1;

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
