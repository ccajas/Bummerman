using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bummerman
{
    using ComponentCollection = Dictionary<ComponentType, Component[]>;

    class SystemManager
    {
        // ECS constants and vars
        int nextEntity = 0;
        const int maxEntities = 1000;

        // Component groups
        Dictionary<ComponentType, Component[]> components;

        /// Manages Entity Components
        ComponentManager componentManager;

        // Entity template/prefab collection
        Dictionary<string, EntityTemplate> entityTemplates;

        // System collection
        List<EntitySystem> entitySystems;

        /// <summary>
        /// Setup lists and component groups
        /// </summary>
        public SystemManager()
        {
            entityTemplates = new Dictionary<string, EntityTemplate>();
            entitySystems = new List<EntitySystem>();

            componentManager = new ComponentManager();

            // Add component dictionary and component arrays to it
            components = new Dictionary<ComponentType, Component[]>();

            // Component arrays get added here
            components.Add(ComponentType.ScreenPosition, new Components.ScreenPosition[maxEntities]);
            components.Add(ComponentType.TilePosition, new Components.TilePosition[maxEntities]);
            components.Add(ComponentType.Sprite, new Components.Sprite[maxEntities]);
            components.Add(ComponentType.InputContext, new Components.InputContext[maxEntities]);
            components.Add(ComponentType.Collision, new Components.Collision[maxEntities]);

            components.Add(ComponentType.PlayerInfo, new Components.PlayerInfo[maxEntities]);
            components.Add(ComponentType.Bomb, new Components.Bomb[maxEntities]);
            components.Add(ComponentType.PowerUp, new Components.PowerUp[maxEntities]);
            components.Add(ComponentType.TimedEffect, new Components.TimedEffect[maxEntities]);
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
            entityTemplates.Add("Player", EntityPrefabs.CreatePlayer());
            entityTemplates.Add("Bomb", EntityPrefabs.CreatePlayer());
            entityTemplates.Add("SolidBlock", EntityPrefabs.CreateSolidBlock());
            entityTemplates.Add("SoftBlock", EntityPrefabs.CreateSoftBlock());
        }

        /// <summary>
        /// Create an entity from a template
        /// </summary>
        /// <param name="templateName"></param>
        public EntityTemplate CreateEntityFromTemplate(string templateName)
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

                // Check every list for proper insertion
                foreach (Component component in newTemplate.componentList)
                    components[component.type][nextEntity] = component;
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
            {
                // Amount of entities might have changed since this step
                nextEntity = system.Process(frameStepTime, nextEntity);
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
    }
}
