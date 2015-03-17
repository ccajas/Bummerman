using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Meteor.ECS;

namespace Bummerman
{
    enum CollisionType
    {
        PassThrough = 0,
        Player,
        SolidBlock,
        SoftBlock,
        SemiSolid,
        Explosion
    }
}

namespace Bummerman.Components
{
    class Collision : Component
    {
        public override ComponentType type { get { return ComponentType.Collision; } }

        public CollisionType collisionType = CollisionType.PassThrough;
        public Rectangle bounds = new Rectangle();
        public Point offset = Point.Zero;
    }
}
