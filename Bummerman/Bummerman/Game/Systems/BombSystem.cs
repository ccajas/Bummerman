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

        /// <summary>
        /// Update and add/remove bombs as needed
        /// </summary>
        public override void Process(TimeSpan frameStepTime, int totalEntities)
        {
            uint messageID = GetMessage(MessageType.Player1State).messageID;

            if (messageID == Convert.ToInt16(InputActions.setBomb))
            {
                for (int i = 0; i < totalEntities; i++)
                {
                    if (components.bomb[i] != null)
                    {
                        components.bomb[i].live = true;
                        components.sprite[i].live = true;
                    }
                }
            }

            base.Process(frameStepTime, totalEntities);
        }
    }
}
