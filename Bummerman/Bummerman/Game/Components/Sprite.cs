using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bummerman.Components
{
    class Sprite : Component
    {
        public override ComponentType type { get { return ComponentType.Sprite; } }

        public string spriteTexture;
        public Rectangle textureArea = new Rectangle();
    }
}
