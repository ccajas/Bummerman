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
        /// Add systems
        /// </summary>
        public void SetupSystems(Dictionary<string, Texture2D> textureCollection)
        {
            // The order in which systems are added makes a difference in 
            // how components interact. The only exception are systems that mainly use Draw().

            entitySystems.Add(new Systems.InputSystem(entityManager));
            entitySystems.Add(new Systems.BombSystem(entityManager));
            entitySystems.Add(new Systems.ExplosionSystem(entityManager));
            entitySystems.Add(new Systems.TileSystem(entityManager));
            entitySystems.Add(new Systems.MovementSystem(entityManager));
            entitySystems.Add(new Systems.CollisionSystem(entityManager));
            entitySystems.Add(new Systems.SpriteRenderSystem(textureCollection, entityManager));

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
