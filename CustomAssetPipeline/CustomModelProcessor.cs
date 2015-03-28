using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.ComponentModel;

// TODO: replace these with the processor input and output types.
using TInput = System.String;
using TOutput = System.String;

namespace CustomAssetPipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "Custom Model - XNA Framework")]
    public class CustomModelProcessor : ModelProcessor
    {
        [DisplayName("Mesh model rotation")]
        [Description("Rotates 3D models while they are loaded, including rotating their animations.")]
        public Vector3 NodeRotation
        {
            get { return nodeRotation; }
            set { nodeRotation = value; }
        }
        private Vector3 nodeRotation;

        /// <summary>
        /// Actual rotation function
        /// </summary>
        public static void RotateAll(NodeContent node, Vector3 rotation)
        {
            Matrix rotate = Matrix.Identity *
                Matrix.CreateRotationX(MathHelper.ToRadians(rotation.X)) *
                Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y)) *
                Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z));
            MeshHelper.TransformScene(node, rotate);
        }
        
        /// <summary>
        /// Process the model
        /// </summary>
        public override ModelContent Process(
            NodeContent input, ContentProcessorContext context)
        {
            RotateAll(input, NodeRotation);
            return base.Process(input, context);
        }
    }
}