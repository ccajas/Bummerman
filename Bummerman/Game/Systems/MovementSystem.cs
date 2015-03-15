using System;
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
            // Process input data from all possible players
            for (int i = 0; i < totalEntities; i++)
            {
                if (playerInfo[i] != null && playerInfo[i].live)
                {
                    InputContext input = components[ComponentType.InputContext][i] as InputContext;

                    float speed = playerInfo[i].speed;

                    // Toggle animation on
                    if (input.currentState > 0)
                        sprites[i].animation = Animation.DualForward;

                    // Move the position based on input
                    if (input.ValueFound<InputStates>(InputStates.MoveLeft))
                    {
                        screenPos[i].position.X -= speed * (float)frameStepTime.TotalSeconds;
                        sprites[i].textureArea.X = 106;
                    }

                    if (input.ValueFound<InputStates>(InputStates.MoveRight))
                    {
                        screenPos[i].position.X += speed * (float)frameStepTime.TotalSeconds;
                        sprites[i].textureArea.X = 161;
                    }

                    if (input.ValueFound<InputStates>(InputStates.MoveUp))
                    {
                        screenPos[i].position.Y -= speed * (float)frameStepTime.TotalSeconds;
                        sprites[i].textureArea.X = 219;
                    }

                    if (input.ValueFound<InputStates>(InputStates.MoveDown))
                    {
                        screenPos[i].position.Y += speed * (float)frameStepTime.TotalSeconds;
                        sprites[i].textureArea.X = 52;
                    }

                    // Round position to whole numbers
                    screenPos[i].position.X = (float)Math.Round(screenPos[i].position.X);
                    screenPos[i].position.Y = (float)Math.Round(screenPos[i].position.Y);
                }
            }

            return base.Process(frameStepTime, totalEntities);
        }
    }
}
