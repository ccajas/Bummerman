using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bummerman
{
    class MovementSystem : EntitySystem
    {
        public MovementSystem(ComponentCollection components) : base(components) { }

        public override void Process(TimeSpan frameStepTime, int totalEntities)
        {
            // Setup component lists
            Components.ScreenPosition[] screenPos = components.screenPosition;
            Components.PlayerInfo[] playerInfo = components.playerInfo;

            int messageID = components.message[0].messageID;

            // Perform actions if message ID isn't 0
            if (messageID >= 0)
            {
                for (int i = 0; i < totalEntities; i++)
                {
                    if (playerInfo[i] != null)
                    {
                        float speed = playerInfo[i].speed;

                        // Move the position based on input
                        if (messageID == Convert.ToInt16(InputStates.MoveLeft))
                            screenPos[i].position.X -= speed *(float)frameStepTime.TotalSeconds;

                        if (messageID == Convert.ToInt16(InputStates.MoveRight))
                            screenPos[i].position.X += speed * (float)frameStepTime.TotalSeconds;

                        if (messageID == Convert.ToInt16(InputStates.MoveUp))
                            screenPos[i].position.Y -= speed * (float)frameStepTime.TotalSeconds;

                        if (messageID == Convert.ToInt16(InputStates.MoveDown))
                            screenPos[i].position.Y += speed * (float)frameStepTime.TotalSeconds;
                    }
                }
                // Reset message ID
                components.message[0].messageID = -1;
            }

            base.Process(frameStepTime, totalEntities);
        }
    }
}
