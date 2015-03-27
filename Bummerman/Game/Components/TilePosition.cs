using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Meteor.ECS;

namespace Bummerman.Components
{
    class TilePosition : Component
    {
        public override Int32 type { get { return (int)ComponentType.TilePosition; } }

        public Point position = Point.Zero;
        public int tileSize = 24;
    }
}
