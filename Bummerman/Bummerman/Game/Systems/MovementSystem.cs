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
            if (message.messageID >= 0)
            {
                for (int i = 0; i < totalEntities; i++)
                {
                    if (playerInfo[i] != null && playerInfo[i].playerNumber == 1)
                    {
                        float speed = playerInfo[i].speed;

                        // Move the position based on input
                        if (message.ValueFound<InputStates>(InputStates.MoveLeft))
                        {
                            screenPos[i].position.X -= speed *(float)frameStepTime.TotalSeconds;
                            sprites[i].textureArea.X = 106;
                            sprites[i].animation = Animation.Looped;
                        }

                        if (message.ValueFound<InputStates>(InputStates.MoveRight))
                        {
                            screenPos[i].position.X += speed * (float)frameStepTime.TotalSeconds;
                            sprites[i].textureArea.X = 161;
                            sprites[i].animation = Animation.Looped;
                        }

                        if (message.ValueFound<InputStates>(InputStates.MoveUp))
                        {
                            screenPos[i].position.Y -= speed * (float)frameStepTime.TotalSeconds;
                            sprites[i].textureArea.X = 219;
                            sprites[i].animation = Animation.Looped;
                        }

                        if (message.ValueFound<InputStates>(InputStates.MoveDown))
                        {
                            screenPos[i].position.Y += speed * (float)frameStepTime.TotalSeconds;
                            sprites[i].textureArea.X = 52;
                            sprites[i].animation = Animation.Looped;
                        }

                        // Round position to whole numbers
                        screenPos[i].position.X = (float)Math.Round(screenPos[i].position.X);
                        screenPos[i].position.Y = (float)Math.Round(screenPos[i].position.Y);
                    }
                }
            }

            return base.Process(frameStepTime, totalEntities);
        }
    }
}
