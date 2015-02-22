using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman
{
    class BombSystem : EntitySystem
    {
        /// <summary>
        /// Constructor to add components
        /// </summary>
        public BombSystem(ComponentCollection components) : base(components) { }

        public override void Process(TimeSpan frameStepTime, int totalEntities)
        {
            int messageID = components.message[0].messageID;

            //if (messageID == InputActions.setBomb)


            base.Process(frameStepTime, totalEntities);
        }
    }
}
