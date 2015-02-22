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

        // Messages dispatched for all systems
        protected static Message[] messages;

        public EntitySystem(ComponentCollection components)
        {
            this.components = components;

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

        public virtual void Process(TimeSpan frameStepTime, int totalEntities)
        {
            this.totalEntities = totalEntities;
        }

        public virtual void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch) { }
    }
}
