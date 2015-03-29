using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Meteor.ECS;

namespace Bummerman.Components
{
    public class Camera : Component
    {
        public override Int32 type { get { return (int)ComponentType.Camera; } }

		public float cameraArc = 0;
        public float targetArc = 0;
		public float cameraRotation = -90;
        public float targetRotation = -90;

        public Vector3 position;
        public Vector3 lookAt;
        public Matrix view;
        public Matrix projection;

        public bool isRendering = true;
        public float fieldOfView = 90;
        public float aspectRatio = 16f/9f;
	}
}


