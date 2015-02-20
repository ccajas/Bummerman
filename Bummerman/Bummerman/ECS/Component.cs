using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman
{
    abstract class Component
    {
        // ID that ties components to an Entity
        protected int entityID = -1;

        // Life of component
        protected bool live = true;

        /// <summary>
        /// Set the entity ID
        /// </summary>
        public void SetOwnerEntity(int ID)
        {
            entityID = ID;
        }
    }
}
