using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bummerman
{
    enum CollisionType
    {
        PassThrough = 0,
        Player,
        SolidBlock,
        SoftBlock,
        Explosion
    }
}

namespace Bummerman.Components
{
    class Collision : Component
    {
        public CollisionType collisionType = CollisionType.PassThrough;
        public Rectangle collisionBox = new Rectangle();
    }
}
