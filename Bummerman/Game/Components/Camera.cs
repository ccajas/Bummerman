using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Meteor.ECS;

namespace Bummerman.Components
{
    public class Camera : Component
    {
        public override Int32 type { get { return (int)ComponentType.Camera; } }

		protected float cameraArc = 0;
		protected float targetArc = 0;
		protected float cameraRotation = -90;
		protected float targetRotation = -90;

		protected Matrix worldMatrix;
		protected Matrix view;
		protected Matrix projection;

		protected Vector3 position;
		protected float fieldOfView;
		protected float aspectRatio;
	}
}


