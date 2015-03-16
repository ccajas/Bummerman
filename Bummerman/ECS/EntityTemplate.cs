using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman
{
    interface IDeepCloneable<T>
    {
        T DeepClone();
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
        /// Make a deep copy of the template
        /// </summary>
        public EntityTemplate DeepClone()
        {
            var clone = (EntityTemplate)this.MemberwiseClone();

            // This makes sure that deeper references are also cloned.
            //clone._foo = _foo.DeepClone();

            // Though you still need to manually clone types that you do not own like
            // lists but you can also turn this into an extension method if you want.
            //clone._lists = _lists.Select(f => f.DeepClone()).ToList();

            // And you can simply call the ToList/ToArray method for lists/arrays
            // of value type entities.
            //clone._valueTypedLists = _valueTypedLists.ToList();
            return clone;
        }
    }
}
