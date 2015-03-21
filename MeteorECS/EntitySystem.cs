using System;
using System.Collections.Generic;

namespace Meteor.ECS
{
    /// <summary>
    /// Base class for all Systems that work on Entities
    /// </summary>
    public abstract class EntitySystem
    {
        protected ComponentManager componentMgr;
        protected Dictionary<ComponentType, Component[]> components;

        protected int totalEntities = 0;

        /// <summary>
        /// Set up the Entity system
        /// </summary>
        public EntitySystem(ComponentManager componentManager)
        {
            this.componentMgr = componentManager;
            this.components = componentManager.components;
        }

        public void UpdateEntityCount()
        {
            this.totalEntities = componentMgr.TotalEntities;
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
        public DrawableEntitySystem(ComponentManager componentManager) : base(componentManager) { }

        /// <summary>
        /// Draw entities
        /// </summary>
        public abstract void Draw();
    }
}
