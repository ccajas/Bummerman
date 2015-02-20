using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman
{
    abstract class Component
    {
        // ID that ties components to an Entity
        protected int ownerEntity = -1;

        // Life of component
        protected bool live = true;
    }
}
