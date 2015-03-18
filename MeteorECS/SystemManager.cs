using System;
using System.Collections.Generic;
using System.Reflection;

namespace Meteor.ECS
{
    public class SystemManager
    {
        /// System collections
        List<EntitySystem> entitySystems;
        List<DrawableEntitySystem> drawableEntitySystems;

        /// Manages Entity Components
        ComponentManager componentManager;

        /// Public accessor to ComponentManager
        public ComponentManager Entities { get { return componentManager; } }

        /// Total entities used by all systems
        public int totalEntities { get { return componentManager.TotalEntities; } }

        /// <summary>
        /// Setup lists and component groups
        /// </summary>
        public SystemManager(Dictionary<ComponentType, Component[]> componentLists)
        {
            entitySystems = new List<EntitySystem>();
            drawableEntitySystems = new List<DrawableEntitySystem>();

            componentManager = new ComponentManager(componentLists);
        }

        /// <summary>
        /// Add systems through an array
        /// </summary>
        public void AddSystems(EntitySystem[] systems)
        {
            // The order in which systems are added makes a difference in how components interact.
            // The only exception are systems that mainly use Draw().

            foreach (EntitySystem system in systems)
            {
                if (system is DrawableEntitySystem)
                    drawableEntitySystems.Add(system as DrawableEntitySystem);
                
                entitySystems.Add(system);
            }
        }

        /// <summary>
        /// Process components with each system
        /// </summary>
        public void ProcessComponents(TimeSpan frameStepTime)
        {
            foreach (EntitySystem system in entitySystems)
            {
                // Amount of entities might have changed since this step
                system.Process(frameStepTime, componentManager.TotalEntities);
                system.UpdateEntityCount();
                componentManager.RemoveEntities();
            }
        }

        /// <summary>
        /// Draw entities with each system
        /// </summary>
        public void DrawEntities()
        {
            foreach (DrawableEntitySystem system in drawableEntitySystems)
                system.Draw();
        }

        /// <summary>
        /// Debug behavior of entities
        /// </summary>
        public void DebugEntities(ref int[] entityInfo)
        {
            componentManager.OutputEntityGraph(ref entityInfo);
        }
    }
}
