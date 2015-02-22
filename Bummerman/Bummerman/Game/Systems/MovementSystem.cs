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

            Message message = GetMessage(MessageType.Player1State);

            // Perform actions if message ID isn't 0
            if (message.messageID >= 0)
            {
                for (int i = 0; i < totalEntities; i++)
                {
                    if (playerInfo[i] != null && playerInfo[i].playerNumber == 0)
                    {
                        float speed = playerInfo[i].speed;

                        // Move the position based on input
                        if (message.ValueFound<InputStates>(InputStates.MoveLeft))
                            screenPos[i].position.X -= speed *(float)frameStepTime.TotalSeconds;

                        if (message.ValueFound<InputStates>(InputStates.MoveRight))
                            screenPos[i].position.X += speed * (float)frameStepTime.TotalSeconds;

                        if (message.ValueFound<InputStates>(InputStates.MoveUp))
                            screenPos[i].position.Y -= speed * (float)frameStepTime.TotalSeconds;

                        if (message.ValueFound<InputStates>(InputStates.MoveDown))
                            screenPos[i].position.Y += speed * (float)frameStepTime.TotalSeconds;
                    }
                }
                // Reset message ID
                message.messageID = 0;
            }

            base.Process(frameStepTime, totalEntities);
        }
    }
}
