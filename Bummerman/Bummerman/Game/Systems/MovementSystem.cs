using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bummerman.Components;

namespace Bummerman.Systems
{
    /// <summary>
    /// Control Entity movements
    /// </summary>
    class MovementSystem : EntitySystem
    {
        /// Important components
        ScreenPosition[] screenPos;
        PlayerInfo[] playerInfo;
        Sprite[] sprites;

        /// <summary>
        /// Constructor to add components
        /// </summary>
        public MovementSystem(EntityManager entityManager)
            : base(entityManager) 
        {
            // Load important components
            screenPos = components[ComponentType.ScreenPosition] as ScreenPosition[];
            playerInfo = components[ComponentType.PlayerInfo] as PlayerInfo[];
            sprites = components[ComponentType.Sprite] as Sprite[];  
        }

        /// <summary>
        /// Handle player movement
        /// </summary>
        public override int Process(TimeSpan frameStepTime, int totalEntities)
        {
            Message message = GetMessage(MessageType.InputState1);

            // Perform actions if message ID isn't 0
            if (message.messageID > 0)
            {
                int entity = (int)message.receiver;

                if (playerInfo[entity] != null)// && playerInfo[entity].playerNumber == 1)
                {
                    float speed = playerInfo[entity].speed;

                    // Move the position based on input
                    if (message.ValueFound<InputStates>(InputStates.MoveLeft))
                    {
                        screenPos[entity].position.X -= speed * (float)frameStepTime.TotalSeconds;
                        sprites[entity].textureArea.X = 106;
                    }

                    if (message.ValueFound<InputStates>(InputStates.MoveRight))
                    {
                        screenPos[entity].position.X += speed * (float)frameStepTime.TotalSeconds;
                        sprites[entity].textureArea.X = 161;
                    }

                    if (message.ValueFound<InputStates>(InputStates.MoveUp))
                    {
                        screenPos[entity].position.Y -= speed * (float)frameStepTime.TotalSeconds;
                        sprites[entity].textureArea.X = 219;
                    }

                    if (message.ValueFound<InputStates>(InputStates.MoveDown))
                    {
                        screenPos[entity].position.Y += speed * (float)frameStepTime.TotalSeconds;
                        sprites[entity].textureArea.X = 52;
                    }

                    // Round position to whole numbers
                    screenPos[entity].position.X = (float)Math.Round(screenPos[entity].position.X);
                    screenPos[entity].position.Y = (float)Math.Round(screenPos[entity].position.Y);

                    sprites[entity].animation = Animation.DualForward;
                }
            }

            return base.Process(frameStepTime, totalEntities);
        }
    }
}
