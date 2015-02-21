using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Bummerman
{
    abstract class EntitySystem
    {
        protected ComponentCollection components;
        protected int totalEntities = 0;

        public EntitySystem(ComponentCollection components)
        {
            this.components = components;
        }

        public virtual void Process(TimeSpan frameStepTime, int totalEntities)
        {
            this.totalEntities = totalEntities;
        }

        public virtual void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch) { }
    }
}
