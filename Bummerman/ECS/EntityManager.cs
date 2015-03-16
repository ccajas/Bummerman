using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bummerman
{
    /// <summary>
    /// Setup, update, create and delete Component groups as entities.
    /// </summary>
    class EntityManager
    {
        /// ECS constants and vars
        int nextEntity = 0;

        /// Component groups
        public Dictionary<ComponentType, Component[]> components { get; private set; }

        /// Entity template/prefab collection
        Dictionary<string, EntityTemplate> entityTemplates;

        /// Entity count accessor
        public int TotalEntities { get { return nextEntity; } }

        /// <summary>
        /// Setup Component groups
        /// </summary>
        public EntityManager(Dictionary<ComponentType, Component[]> componentLists)
        {
            entityTemplates = new Dictionary<string, EntityTemplate>();

            // Add component dictionary and component arrays to it
            components = new Dictionary<ComponentType, Component[]>();

            foreach (KeyValuePair<ComponentType, Component[]> componentList in componentLists)
                components.Add(componentList.Key, componentList.Value);
        }

        /// <summary>
        /// Wrapper to add Entity templates
        /// </summary>
        public void AddEntityTemplate(string templateName, EntityTemplate template)
        {
            entityTemplates.Add(templateName, template);
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
                // Call method to create new template using the next available ID
                EntityTemplate copyTemplate = entityTemplates[templateName];
                newTemplate = copyTemplate.DeepClone(nextEntity);

                // Add each component
                foreach (Component component in newTemplate.componentList)
                    components[component.type][nextEntity] = component;
            }

            // Finish adding components for entity
            nextEntity++;

            return newTemplate;
        }

        /// <summary>
        /// Set the entity ID for each component here
        /// </summary>
        private EntityTemplate SetComponentEntityIDs(EntityTemplate template, int ID)
        {
            foreach (Component component in template.componentList)
                component.SetOwnerEntity(ID);

            return template;
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
            int totalEntities = nextEntity;
            for (int entity = totalEntities - 1; entity >= 0; --entity)
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
                    foreach (Component[] componentArray in components.Values)
                    {
                        // Update entity IDs
                        componentArray[entity] = componentArray[lastEntityID];
                        componentArray[lastEntityID] = null;

                        if (componentArray[entity] != null)
                            componentArray[entity].SetOwnerEntity(entity);
                    }

                    // Reduce entity count
                    nextEntity--;
                }
            }
            // Finished removing entities
        }

        [Conditional("DEBUG")]
        public void DebugEntityGraph(Viewport viewport, SpriteBatch spriteBatch, Texture2D pixel)
        {
            spriteBatch.Draw(pixel, new Rectangle(0, viewport.Height - 88, viewport.Width, 108), new Color(0, 0, 0, 0.8f));
            spriteBatch.Draw(pixel, new Rectangle(4 + (nextEntity * 4), viewport.Height - 88, 3, 7), Color.White);

            Color[] colors = { new Color(0, 0, 0, 0.5f), Color.White, Color.Blue, Color.Cyan, Color.LightGreen, 
                                 Color.Yellow, Color.Green, Color.Red, Color.Orange, Color.Fuchsia, 
                                 new Color(0, 255, 0), Color.LightSkyBlue };

            // Debug the first 200 entities
            for (int i = 0; i < 200; i++)
            {
                int j = 0;
                foreach (Component[] componentArray in components.Values)
                {
                    int index = 0;
                    if (componentArray[i] != null)
                        index = Convert.ToInt16(componentArray[i].type);

                    spriteBatch.Draw(pixel, new Rectangle(4 + (i * 4), viewport.Height - 80 + (j * 6), 3, 5), colors[index]);

                    j++;
                }
            }
            // Finish debugging entities
        }
    }
}
