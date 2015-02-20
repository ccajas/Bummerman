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

        /// <summary>
        /// Setup component lists
        /// </summary>
        public EntityManager()
        {
            entityTemplates = new Dictionary<string, EntityTemplate>();

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
        /// Create entity templates
        /// </summary>
        public void CreateTemplates(Dictionary<string, Texture2D> textureCollection)
        {
            // Load templates
            entityTemplates.Add(
                "SolidBlocks", 
                EntityPrefabs.CreateSolidBlock()
            );

            entityTemplates.Add(
                "SoftBlock",
                EntityPrefabs.CreateSoftBlock()
            );
        }

        public delegate EntityTemplate TemplateDelegate(string s);

        /// <summary>
        /// Create an entity from a template
        /// </summary>
        /// <param name="templateName"></param>
        public void CreateEntity(string templateName)
        {
            EntityTemplate template = null;

            if (entityTemplates.TryGetValue(templateName, out template))
            {
                Type prefabsType = typeof(EntityPrefabs);
                MethodInfo theMethod = prefabsType.GetMethod("Create" + templateName);

                // Call proper EntityPrefab method to create new template
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
        }

        /// <summary>
        /// Simple deep clone procedure
        /// </summary>
        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
