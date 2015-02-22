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
        Message,
        PlayerInfo,
        PowerUp,
        ScreenPosition,
        Sprite,
        TilePosition
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
