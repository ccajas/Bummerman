﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Meteor.ECS;

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
        public override Int32 type { get { return (int)ComponentType.Sprite; } }

        public string spriteTexture;
        public bool billboard = false;
        public Point textureSize = Point.Zero;
        public Rectangle textureArea = new Rectangle();
        public Animation animation = Animation.None;
        public float frameLength = 1f;
        public float frameTime = 0f;
        public int frameCount = 1;
        public int frame = 0;
    }
}
