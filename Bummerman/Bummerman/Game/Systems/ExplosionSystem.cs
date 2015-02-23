using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bummerman.Components;

namespace Bummerman.Systems
{
    /// <summary>
    /// Updates explosions and handles their effects
    /// </summary>
    class ExplosionSystem : EntitySystem
    {
        /// Important components
        TilePosition[] tiles;
        Collision[] colliders;

        /// <summary>
        /// Constructor to add component references
        /// </summary>
        public ExplosionSystem(EntityManager entityManager)
            : base(entityManager)
        {
            // Load important components
            tiles = components[ComponentType.TilePosition] as TilePosition[];
            colliders = components[ComponentType.Collision] as Collision[];
        }
    }
}
