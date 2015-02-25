using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bummerman
{
    /// <summary>
    /// Setup, update, create and delete Component groups as entities.
    /// </summary>
    class EntityManager
    {
        /// ECS constants and vars
        int nextEntity = 0;
        const int maxEntities = 1000;

        /// Component groups
        public Dictionary<ComponentType, Component[]> components { get; private set; }

        /// Entity template/prefab collection
        Dictionary<string, EntityTemplate> entityTemplates;

        /// Entity count accessor
        public int TotalEntities { get { return nextEntity; } }

        /// <summary>
        /// Setup Component groups
        /// </summary>
        public EntityManager()
        {
            entityTemplates = new Dictionary<string, EntityTemplate>();

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
            components.Add(ComponentType.Spreadable, new Components.Spreadable[maxEntities]);
            components.Add(ComponentType.TimedEffect, new Components.TimedEffect[maxEntities]);
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
            entityTemplates.Add("Explosion", EntityPrefabs.CreateExplosion());
            entityTemplates.Add("PowerUp_ExtraBomb", EntityPrefabs.CreatePowerUp_ExtraBomb());
        }

        /// <summary>
        /// Create an entity from a template
        /// </summary>
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
        /// Disable an entity
        /// </summary>
        public void DisableEntity(int entityID)
        {
            // Check every list for proper insertion
            foreach (Component[] componentArray in components.Values)
            {
                if (componentArray[entityID] != null)
                    componentArray[entityID].live = false;
            }
        }

        /// <summary>
        /// Remove entities that are not considered Live anymore
        /// </summary>
        public void RemoveEntities()
        {
            for (int entity = 0; entity < nextEntity; entity++)
            {
                bool entityToRemove = true;

                // Check every array if all components aren't live
                foreach (Component[] componentArray in components.Values)
                {
                    if (componentArray[entity] != null && componentArray[entity].live)
                        entityToRemove = false;
                }

                if (entityToRemove)
                {
                    // Overwrite this entity's compnents
                    int lastEntityID = nextEntity - 1;
                    if (entity != lastEntityID)
                    {
                        var keys = new List<ComponentType>(components.Keys);
                        foreach (Component[] componentArray in components.Values)
                        {
                            // Update entity IDs
                            componentArray[entity] = componentArray[lastEntityID];
                            if (componentArray[entity] != null)
                                componentArray[entity].SetOwnerEntity(entity);
                        }
                    }

                    // Reduce entity count, and move back one to avoid skipping next entity
                    nextEntity--;
                    entity--;
                }
            }
            // Finished removing entities
        }
    }
}
