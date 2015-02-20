using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman
{
    class EntityTemplate
    {
        public List<Component> componentList { get; private set; }
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
    }
}
