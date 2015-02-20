using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bummerman
{
    class EntityManager
    {
        // ECS constants and vars
        int nextEntity = 0;
        const int maxEntities = 1000;

        // Component groups
        Components.ScreenPosition[] screenPositionComponents;
        Components.TilePosition[] tilePositionComponents;
        Components.Sprite[] spriteComponents;
        Components.Collision[] collisionComponents;
        Components.PlayerInfo[] playerInfoComponents;
        Components.PowerUp[] powerUpComponents;
        Components.InputContext[] inputComponents;

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

            // Setup component lists
            screenPositionComponents = new Components.ScreenPosition[maxEntities];
            tilePositionComponents = new Components.TilePosition[maxEntities];
            spriteComponents = new Components.Sprite[maxEntities];
            collisionComponents = new Components.Collision[maxEntities];
            playerInfoComponents = new Components.PlayerInfo[maxEntities];
            powerUpComponents = new Components.PowerUp[maxEntities];
            inputComponents = new Components.InputContext[maxEntities];
        }

        /// <summary>
        /// Add systems
        /// </summary>
        public void SetupSystems(Dictionary<string, Texture2D> textureCollection)
        {
            entitySystems.Add(new SpriteRenderSystem(textureCollection,
                spriteComponents, screenPositionComponents));
            entitySystems.Add(new TileSystem(tilePositionComponents, 
                screenPositionComponents));
        }

        /// <summary>
        /// Create entity templates
        /// </summary>
        public void CreateTemplates()
        {
            // Load templates
            entityTemplates.Add(
                "SolidBlock", 
                EntityPrefabs.CreateSolidBlock()
            );

            entityTemplates.Add(
                "SoftBlock",
                EntityPrefabs.CreateSoftBlock()
            );

            entityTemplates.Add(
                "Player",
                EntityPrefabs.CreatePlayer()
            );
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
                    if (component is Components.Collision)
                        collisionComponents[nextEntity] = (component as Components.Collision);

                    if (component is Components.InputContext)
                        inputComponents[nextEntity] = (component as Components.InputContext);

                    if (component is Components.PlayerInfo)
                        playerInfoComponents[nextEntity] = (component as Components.PlayerInfo);

                    if (component is Components.PowerUp)
                        powerUpComponents[nextEntity] = (component as Components.PowerUp);

                    if (component is Components.ScreenPosition)
                        screenPositionComponents[nextEntity] = (component as Components.ScreenPosition);

                    if (component is Components.Sprite)
                        spriteComponents[nextEntity] = (component as Components.Sprite);

                    if (component is Components.TilePosition)
                        tilePositionComponents[nextEntity] = (component as Components.TilePosition);
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
