using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Meteor.ECS;

namespace Bummerman.Components
{
    class MeshModel : Component
    {
        public override Int32 type { get { return (int)ComponentType.MeshModel; } }

        public string modelName;
        public Vector3 position = Vector3.Zero;
        public Matrix matrix = Matrix.Identity;
    }
}
