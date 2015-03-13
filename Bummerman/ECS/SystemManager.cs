using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;

namespace Bummerman
{
    class SystemManager
    {
        /// System collection
        List<EntitySystem> entitySystems;

        /// Manages Entity Components
        EntityManager entityManager;

        /// Public accessor to ComponentManager
        public EntityManager Entities { get { return entityManager; } }

        /// Total entities used by all systems
        public int totalEntities { get { return entityManager.TotalEntities; } }

        /// <summary>
        /// Setup lists and component groups
        /// </summary>
        public SystemManager()
        {
            entitySystems = new List<EntitySystem>();
            entityManager = new EntityManager();
        }

        /// <summary>
        /// Add systems through an array
        /// </summary>
        public void AddSystems(EntitySystem[] systems)
        {
            // The order in which systems are added makes a difference in how components interact.
            // The only exception are systems that mainly use Draw().

            foreach (EntitySystem system in systems)
                entitySystems.Add(system);

            entityManager.CreateTemplates();
        }

        /// <summary>
        /// Process components with each system
        /// </summary>
        public void ProcessComponents(TimeSpan frameStepTime)
        {
            foreach (EntitySystem system in entitySystems)
            {
                // Amount of entities might have changed since this step
                system.Process(frameStepTime, entityManager.TotalEntities);
                system.UpdateEntityCount();
                entityManager.RemoveEntities();
            }
        }

        /// <summary>
        /// Draw entities with each system
        /// </summary>
        public void DrawEntities(SpriteBatch spriteBatch)
        {
            foreach (EntitySystem system in entitySystems)
                system.Draw(spriteBatch.GraphicsDevice, spriteBatch);
        }

        public void DebugEntities(Viewport viewport, SpriteBatch spriteBatch, Texture2D pixel)
        {
            entityManager.DebugEntityGraph(viewport, spriteBatch, pixel);
        }
    }
}
