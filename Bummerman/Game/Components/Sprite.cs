using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bummerman
{
    enum Animation
    {
        None,
        Forward,
        Reverse,
        DualForward,
        DualReverse,
        NotLooped
    }
}

namespace Bummerman.Components
{
    class Sprite : Component
    {
        public override ComponentType type { get { return ComponentType.Sprite; } }

        public string spriteTexture;
        public Rectangle textureArea = new Rectangle();
        public Animation animation = Animation.None;
        public float frameLength = 1f;
        public float frameTime = 0f;
        public int frameCount = 1;
        public int frame = 0;
    }
}
