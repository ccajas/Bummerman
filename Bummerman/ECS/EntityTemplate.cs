using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman
{
    interface IDeepCloneable<T>
    {
        T DeepClone(int entityID);
    }

    class EntityTemplate : IDeepCloneable<EntityTemplate>
    {
        public List<Component> componentList { get; set; }
        public int componentMask { get; private set; }
        string name;

        /// <summary>
        /// Create a template from components
        /// </summary>
        public EntityTemplate(string templateName, params Component[] components)
        {
            componentList = new List<Component>();
            name = templateName;

            foreach (Component component in components)
            {
                componentList.Add(component);
                componentMask |= Convert.ToInt32(component.type);
            }    
        }

        /// <summary>
        /// Get component based on type
        /// </summary>
        public Component GetComponent(ComponentType type)
        {
            int componentType = Convert.ToInt32(type);

            if ((componentMask & componentType) == componentType)
            {
                foreach (Component component in componentList)
                {
                    if (component.type == type)
                        return component;
                }
            }
            return null;
        }

        /// <summary>
        /// Make a deep copy of the template, with entity ID
        /// </summary>
        public EntityTemplate DeepClone(int entityID = -1)
        {
            var clone = (EntityTemplate)this.MemberwiseClone();

            // Clone list of components
            clone.componentList = clone.componentList.Select(f => f.DeepClone(entityID)).ToList();

            // And you can simply call the ToList/ToArray method for lists/arrays
            // of value type entities.
            //clone._valueTypedLists = _valueTypedLists.ToList();
            return clone;
        }
    }
}
