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

        // Messages dispatched for all systems
        protected static Message[] messages;

        public EntitySystem(EntityManager entityManager)
        {
            this.entityMgr = entityManager;
            this.components = entityManager.components;

            var messageTypeCount = Enum.GetNames(typeof(MessageType)).Length;
            messages = new Message[messageTypeCount];

            // Setup default message states
            for (int i = 0; i < messageTypeCount; i++)
                messages[i] = new Message();
        }

        /// <summary>
        /// Get the message according to Message Type ID
        /// </summary>
        protected Message GetMessage(MessageType type)
        {
            return EntitySystem.messages[Convert.ToInt16(type)];
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
