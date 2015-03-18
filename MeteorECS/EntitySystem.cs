using System;
using System.Collections.Generic;

namespace Meteor.ECS
{
    /// <summary>
    /// Base class for all Systems that work on Entities
    /// </summary>
    public abstract class EntitySystem
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
    /// An Entity System for Entities that need to be drawn
    /// </summary>
    public abstract class DrawableEntitySystem : EntitySystem
    {
        /// <summary>
        /// Overloaded constructor
        /// </summary>
        public DrawableEntitySystem(EntityManager entityManager) : base(entityManager) { }

        /// <summary>
        /// Draw entities
        /// </summary>
        public abstract void Draw();
    }
}
