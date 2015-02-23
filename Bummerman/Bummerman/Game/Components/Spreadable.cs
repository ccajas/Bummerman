using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman.Components
{
    class Spreadable : Component
    {
        public override ComponentType type { get { return ComponentType.Bomb; } }
    }
}
