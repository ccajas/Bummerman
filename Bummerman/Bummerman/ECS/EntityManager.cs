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
        int entityCount = 0;
        const int maxEntities = 2000;
        const int maxComponents = 250;

        // Component groups
        List<Components.ScreenPosition> screenPositionComponents;
        List<Components.TilePosition> tilePositionComponents;
        List<Components.Sprite> spriteComponents;
        List<Components.Collision> collisionComponents;
        List<Components.PlayerInfo> playerInfoComponents;
        List<Components.PowerUp> powerUpComponents;
        List<Components.InputContext> inputComponents;

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
            screenPositionComponents = new List<Components.ScreenPosition>(maxComponents);
            tilePositionComponents = new List<Components.TilePosition>(maxComponents);
            spriteComponents = new List<Components.Sprite>(maxComponents);
            collisionComponents = new List<Components.Collision>(maxComponents);
            playerInfoComponents = new List<Components.PlayerInfo>(maxComponents);
            powerUpComponents = new List<Components.PowerUp>(maxComponents);
            inputComponents = new List<Components.InputContext>(10);
        }

        /// <summary>
        /// Add systems
        /// </summary>
        public void SetupSystems(Dictionary<string, Texture2D> textureCollection)
        {
            entitySystems.Add(new SpriteRenderSystem(textureCollection,
                spriteComponents, screenPositionComponents));
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
        }

        /// <summary>
        /// Create an entity from a template
        /// </summary>
        /// <param name="templateName"></param>
        public void CreateEntity(string templateName)
        {
            EntityTemplate template = null;

            // Check if a valid template exists first
            if (entityTemplates.TryGetValue(templateName, out template))
            {
                // Get proper EntityPrefab method
                Type prefabsType = typeof(EntityPrefabs);
                MethodInfo theMethod = prefabsType.GetMethod("Create" + templateName);

                // Call method to create new template
                EntityTemplate newTemplate = (EntityTemplate)theMethod.Invoke(null, null);

                // Do a brute force test for type checking to insert in proper list
                // (could be improved)
                foreach (Component component in newTemplate.componentList)
                {                 
                    if (component is Components.Collision)
                        collisionComponents.Add((component as Components.Collision));

                    if (component is Components.InputContext)
                        inputComponents.Add((component as Components.InputContext));

                    if (component is Components.PlayerInfo)
                        playerInfoComponents.Add((component as Components.PlayerInfo));

                    if (component is Components.PowerUp)
                        powerUpComponents.Add((component as Components.PowerUp));

                    if (component is Components.ScreenPosition)
                        screenPositionComponents.Add((component as Components.ScreenPosition));

                    if (component is Components.Sprite)
                        spriteComponents.Add((component as Components.Sprite));

                    if (component is Components.TilePosition)
                        tilePositionComponents.Add((component as Components.TilePosition));
                }
            }

            // Finish adding components for entity
            entityCount++;
        }

        public void UpdateSystems()
        {

        }

        public void DrawSystems(GraphicsDevice graphicsDevice)
        {
            foreach (EntitySystem system in entitySystems)
                system.Draw(graphicsDevice);
        }
    }
}
