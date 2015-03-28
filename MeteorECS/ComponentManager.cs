using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;

namespace Meteor.ECS
{
    /// <summary>
    /// Setup, update, create and delete Component groups as entities.
    /// </summary>
    public class ComponentManager
    {
        /// ECS constants and vars
        int nextEntity = 0;

        /// Component groups
        public Dictionary<Int32, Component[]> components { get; private set; }

        /// Entity template/prefab collection
        Dictionary<string, EntityTemplate> entityTemplates;

        /// Entity count accessor
        public int TotalEntities { get { return nextEntity; } }

        /// <summary>
        /// Setup Component groups
        /// </summary>
        public ComponentManager(Dictionary<Int32, Component[]> componentLists)
        {
            entityTemplates = new Dictionary<string, EntityTemplate>();

            // Add component dictionary and component arrays to it
            components = new Dictionary<Int32, Component[]>();

            foreach (KeyValuePair<Int32, Component[]> componentList in componentLists)
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
        /// Create an entity from a template
        /// </summary>
        public EntityTemplate CreateEntityFromTemplate(EntityTemplate newTemplate)
        {
            // Add each component
            foreach (Component component in newTemplate.componentList)
                components[component.type][nextEntity] = component;

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

        /// <summary>
        /// Produce bitmask component for all Entities
        /// </summary>
        [Conditional("DEBUG")]
        public void OutputEntityGraph(ref int[] entityInfo)
        {
            // Resize info when needed
            if (nextEntity != entityInfo.Length)
                Array.Resize<int>(ref entityInfo, nextEntity);

            // Debug the active entities
            for (int i = 0; i < nextEntity; i++)
            {
                foreach (Component[] componentArray in components.Values)
                {
                    if (componentArray[i] != null)
                        entityInfo[i] |= 1 << Convert.ToInt16(componentArray[i].type);
                }
            }
            // Finish outputting entities
        }
    }
}
