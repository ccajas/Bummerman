using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bummerman.Components
{
    class TilePosition : Component
    {
        public override ComponentType type { get { return ComponentType.TilePosition; } }

        public Point position = Point.Zero;
        public int tileSize = 0;
    }
}
