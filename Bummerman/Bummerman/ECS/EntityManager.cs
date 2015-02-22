using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bummerman
{
    // Component groups
    class ComponentCollection
    {
        public Components.ScreenPosition[] screenPosition;
        public Components.TilePosition[] tilePosition;
        public Components.Sprite[] sprite;
        public Components.Collision[] collision;
        public Components.PlayerInfo[] playerInfo;
        public Components.Bomb[] bomb;
        public Components.PowerUp[] powerUp;
        public Components.InputContext[] inputContext;
        public Components.TimedEffect[] timedEffect;
    }

    class EntityManager
    {
        // ECS constants and vars
        int nextEntity = 0;
        const int maxEntities = 1000;

        // Create component collection
        ComponentCollection components;

        // Entity template/prefab collection
        Dictionary<string, EntityTemplate> entityTemplates;

        // System collection
        List<EntitySystem> entitySystems;

        /// <summary>
        /// Setup lists and component groups
        /// </summary>
        public EntityManager()
        {
            entityTemplates = new Dictionary<string, EntityTemplate>();
            entitySystems = new List<EntitySystem>();

            components = new ComponentCollection();

            // Setup component lists
            components.screenPosition = new Components.ScreenPosition[maxEntities];
            components.tilePosition = new Components.TilePosition[maxEntities];
            components.sprite = new Components.Sprite[maxEntities];
            components.inputContext = new Components.InputContext[maxEntities];
            components.collision = new Components.Collision[maxEntities];
            components.playerInfo = new Components.PlayerInfo[maxEntities];
            components.bomb = new Components.Bomb[maxEntities];
            components.powerUp = new Components.PowerUp[maxEntities];
            components.timedEffect = new Components.TimedEffect[maxEntities];
        }

        /// <summary>
        /// Add systems
        /// </summary>
        public void SetupSystems(Dictionary<string, Texture2D> textureCollection)
        {
            // The order in which systems are added makes a difference in 
            // how components interact. The only exception are systems that mainly use Draw().

            entitySystems.Add(new Systems.SpriteRenderSystem(textureCollection, components));
            entitySystems.Add(new Systems.InputSystem(components));
            entitySystems.Add(new Systems.BombSystem(components));
            entitySystems.Add(new Systems.TileSystem(components));
            entitySystems.Add(new Systems.MovementSystem(components));
            entitySystems.Add(new Systems.CollisionSystem(components));
        }

        /// <summary>
        /// Create entity templates
        /// </summary>
        public void CreateTemplates()
        {
            // Load templates
            entityTemplates.Add("Player",
                EntityPrefabs.CreatePlayer());

            entityTemplates.Add("Bomb",
                EntityPrefabs.CreatePlayer());

            entityTemplates.Add("SolidBlock",
                EntityPrefabs.CreateSolidBlock());

            entityTemplates.Add("SoftBlock",
                EntityPrefabs.CreateSoftBlock());
        }

        /// <summary>
        /// Create an entity from a template
        /// </summary>
        /// <param name="templateName"></param>
        public EntityTemplate CreateEntity(string templateName)
        {
            EntityTemplate template = null;
            EntityTemplate newTemplate = null;

            // Check if a valid template exists first
            if (entityTemplates.TryGetValue(templateName, out template))
            {
                // Get proper EntityPrefab method
                Type prefabsType = typeof(EntityPrefabs);
                MethodInfo theMethod = prefabsType.GetMethod("Create" + templateName);

                // Call method to create new template
                newTemplate = (EntityTemplate)theMethod.Invoke(null, new object[] { nextEntity });

                // Check every list for proper insertion (could be improved)
                foreach (Component component in newTemplate.componentList)
                {
                    if (component is Components.Bomb)
                        components.bomb[nextEntity] = (component as Components.Bomb);

                    if (component is Components.Collision)
                        components.collision[nextEntity] = (component as Components.Collision);

                    if (component is Components.InputContext)
                        components.inputContext[nextEntity] = (component as Components.InputContext);

                    if (component is Components.PlayerInfo)
                        components.playerInfo[nextEntity] = (component as Components.PlayerInfo);

                    if (component is Components.PowerUp)
                        components.powerUp[nextEntity] = (component as Components.PowerUp);

                    if (component is Components.ScreenPosition)
                        components.screenPosition[nextEntity] = (component as Components.ScreenPosition);

                    if (component is Components.Sprite)
                        components.sprite[nextEntity] = (component as Components.Sprite);

                    if (component is Components.TilePosition)
                        components.tilePosition[nextEntity] = (component as Components.TilePosition);

                    if (component is Components.TimedEffect)
                        components.timedEffect[nextEntity] = (component as Components.TimedEffect);
                }
            }

            // Finish adding components for entity
            nextEntity++;

            return newTemplate;
        }

        /// <summary>
        /// Process components with each system
        /// </summary>
        public void ProcessComponents(TimeSpan frameStepTime)
        {
            foreach (EntitySystem system in entitySystems)
                system.Process(frameStepTime, nextEntity);
        }

        /// <summary>
        /// Draw entities with each system
        /// </summary>
        public void DrawEntities(SpriteBatch spriteBatch)
        {
            foreach (EntitySystem system in entitySystems)
                system.Draw(spriteBatch.GraphicsDevice, spriteBatch);
        }
    }
}
