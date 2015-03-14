using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Bummerman
{
    /// Alias type for shorthand dictionary of Components
    using ComponentCollection = Dictionary<ComponentType, Component[]>;

    /// <summary>
    /// Base class for all Systems that work on Entities
    /// </summary>
    abstract class EntitySystem
    {
        protected EntityManager entityMgr;
        protected ComponentCollection components;

        protected int totalEntities = 0;

        /// <summary>
        /// Set up the Entity system
        /// </summary>
        public EntitySystem(EntityManager entityManager)
        {
            this.entityMgr = entityManager;
            this.components = entityManager.components;
        }

        public void UpdateEntityCount()
        {
            this.totalEntities = entityMgr.TotalEntities;
        }

        public virtual int Process(TimeSpan frameStepTime, int totalEntities)
        {
            return this.totalEntities;
        }

        public virtual void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch) { }
    }
}
