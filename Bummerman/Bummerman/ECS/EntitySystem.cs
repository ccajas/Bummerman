using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Bummerman
{
    abstract class EntitySystem
    {
        public abstract void Process();
        public virtual void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch) { }
    }
}
