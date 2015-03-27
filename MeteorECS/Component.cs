using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteor.ECS
{
    public abstract class Component : IDeepCloneable<Component>
    {
        // ID that ties components to an Entity
        public int entityID { get; private set; }

        // Life of component
        public bool live = true;

        // Component type ID
        public abstract Int32 type { get; }

        /// <summary>
        /// Set the entity ID
        /// </summary>
        public void SetOwnerEntity(int ID)
        {
            entityID = ID;
        }

        /// <summary>
        /// Simply clone component properties
        /// </summary>
        public Component DeepClone(int ID)
        {
            // Deep clone works for value types only
            SetOwnerEntity(ID);
            return (Component)this.MemberwiseClone();
        }
    }
}
