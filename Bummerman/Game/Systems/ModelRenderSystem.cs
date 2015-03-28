using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bummerman.Components;
using Meteor.ECS;

namespace Bummerman
{
    class ModelRenderSystem : DrawableEntitySystem
    {
        /// Reference to texture and model assets
        Dictionary<string, Model> modelCollection;
        Dictionary<string, Texture2D> textureCollection;

        /// Important components
        MeshModel[] models;
        Camera[] cameras;

        /// <summary>
        /// Constructor to add component references
        /// </summary>
        public ModelRenderSystem(ComponentManager componentManager,
            Dictionary<string, Model> modelCollection,
            Dictionary<string, Texture2D> textureCollection)
            : base(componentManager)
        {
            // Initialize asset collections
            this.textureCollection = textureCollection;
            this.modelCollection = modelCollection;

            // Load important components
            models = components[(int)ComponentType.MeshModel] as MeshModel[];
            cameras = components[(int)ComponentType.Camera] as Camera[];
        }

        /// <summary>
        /// Draw entities with Model components
        /// </summary>     
        public override void Draw()
        {

        }
    }
}
