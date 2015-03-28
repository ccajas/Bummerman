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
        Dictionary<string, Effect> effectCollection;
        Dictionary<string, Texture2D> textureCollection;

        /// Important components
        MeshModel[] models;
        Camera[] cameras;

        /// <summary>
        /// Constructor to add component references
        /// </summary>
        public ModelRenderSystem(ComponentManager componentManager,
            Dictionary<string, Model> modelCollection,
            Dictionary<string, Effect> effectCollection,
            Dictionary<string, Texture2D> textureCollection)
            : base(componentManager)
        {
            // Initialize asset collections
            this.textureCollection = textureCollection;
            this.modelCollection = modelCollection;
            this.effectCollection = effectCollection;

            // Load important components
            models = components[(int)ComponentType.MeshModel] as MeshModel[];
            cameras = components[(int)ComponentType.Camera] as Camera[];
        }

        /// <summary>
        /// Draw entities with Model components
        /// </summary>     
        public override void Draw()
        {
            for (int i = 0; i < totalEntities; i++)
            {
                // Check valid Model components
                if (cameras[i] != null)
                {
                    // Set global shader variables
                    Effect defaultEffect = effectCollection["default"];

                    defaultEffect.Parameters["View"].SetValue(cameras[i].view);
                    defaultEffect.Parameters["Projection"].SetValue(cameras[i].projection);
                }
            }

            for (int i = 0; i < totalEntities; i++)
            {
                // Check valid Model components
                if (models[i] != null && models[i].live)
                {
                    Model model = modelCollection[models[i].modelName];
                    Effect defaultEffect = effectCollection[models[i].effectName];

                    // Set model position
                    defaultEffect.Parameters["World"].SetValue(models[i].matrix);

                    foreach (ModelMesh mesh in model.Meshes)
                    {
                        foreach (ModelMeshPart part in mesh.MeshParts)
                            part.Effect = defaultEffect;

                        // Draw the mesh with a particular effect
                        foreach (Effect effect in mesh.Effects)
                        {
                            effect.CurrentTechnique = effect.Techniques["Technique1"];
                            mesh.Draw();
                        }
                    }
                    // Finish drawing this model
                }

            }
            // Finish drawing all models
        }
    }
}
