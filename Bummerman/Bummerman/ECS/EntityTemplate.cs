using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman
{
    class EntityTemplate
    {
        public List<Component> componentList { get; private set; }
        string name;

        public EntityTemplate(string templateName, params Component[] components)
        {
            componentList = new List<Component>();
            name = templateName;

            foreach (Component component in components)
                componentList.Add(component);
        }
    }
}
