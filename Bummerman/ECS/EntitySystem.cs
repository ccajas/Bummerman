using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Meteor.ECS
{
    /// <summary>
    /// Base class for all Systems that work on Entities
    /// </summary>
    abstract class EntitySystem
    {
        protected EntityManager entityMgr;
        protected Dictionary<ComponentType, Component[]> components;

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

        /// <summary>
        /// Process entities
        /// </summary>
        public virtual int Process(TimeSpan frameStepTime, int totalEntities)
        {
            return this.totalEntities;
        }
    }

    /// <summary>
    /// A EntitySystem that supports drawing of Entities
    /// </summary>
    abstract class DrawableEntitySystem : EntitySystem
    {
        /// <summary>
        /// Set up the Entity system
        /// </summary>
        public DrawableEntitySystem(EntityManager entityManager) : base(entityManager) { }

        /// <summary>
        /// Draw entities
        /// </summary>
        public abstract void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch);
    }
}
