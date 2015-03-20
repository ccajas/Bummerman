using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lidgren.Network;
using Meteor.ECS;

namespace Bummerman.ScreenElements
{
    partial class GameScreen : DrawableScreenElement
    {
        // ECS entity manager
        SystemManager systemManager;

        // Sprite textures and other assets
        Dictionary<string, Texture2D> textureCollection;
        SpriteFont debugFont;
        RenderTarget2D screenRT;

        // Game resources
        Level level;

        // Virtual resolution for adaptive resizing
        int virtualBufferWidth = 428;
        int virtualBufferHeight = 240;

        // Default to virtual res ratio
        float virtualResolutionRatio = 1f;

        /// <summary>
        /// Setup ECS framework for game entities
        /// </summary>
        public GameScreen(Game game, ScreenElement previousScreenElement) : 
            base(previousScreenElement, game.GraphicsDevice)
        {
            // Setup game assets
            textureCollection = new Dictionary<string, Texture2D>();
            textureCollection.Add("sprites", game.Content.Load<Texture2D>("textures/sprites"));
            debugFont = game.Content.Load<SpriteFont>("debug");

            this.game = game;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(graphicsDevice);

            // Set render target to virtual resolution
            screenRT = new RenderTarget2D(graphicsDevice,
                virtualBufferWidth,
                virtualBufferHeight
            );
            virtualResolutionRatio = (float)graphicsDevice.Viewport.Width / (float)virtualBufferWidth;

            // Add SystemManager and component types to it
            SetupComponentsAndSystems();
            SetupEntityPrefabs();

            // Setup level
            level = new Level();

            // Load level entities
            level.Load(systemManager.Entities);
        }

        /// <summary>
        /// Creates systems and entity templates
        /// </summary>
        private void SetupComponentsAndSystems()
        {
            int maxEntities = 1000;

            systemManager = new SystemManager(new Dictionary<ComponentType, Component[]>
            { 
                { ComponentType.ScreenPosition, new Components.ScreenPosition[maxEntities]},
                { ComponentType.TilePosition,   new Components.TilePosition[maxEntities]},
                { ComponentType.Sprite,         new Components.Sprite[maxEntities]},
                { ComponentType.InputContext,   new Components.InputContext[maxEntities]},
                { ComponentType.Collision,      new Components.Collision[maxEntities]},
                { ComponentType.PlayerInfo,     new Components.PlayerInfo[maxEntities]},
                { ComponentType.Bomb,           new Components.Bomb[maxEntities]},
                { ComponentType.PowerUp,        new Components.PowerUp[maxEntities]},
                { ComponentType.Spreadable,     new Components.Spreadable[maxEntities]},
                { ComponentType.TimedEffect,    new Components.TimedEffect[maxEntities]}
            });

            systemManager.AddSystems(new EntitySystem[] 
            {
                new InputSystem         (systemManager.Entities),
                new MovementSystem      (systemManager.Entities),
                new BombSystem          (systemManager.Entities),
                new ExplosionSystem     (systemManager.Entities),
                new PowerUpSystem       (systemManager.Entities),
                new TileSystem          (systemManager.Entities),
                new CollisionSystem     (systemManager.Entities),
                new SpriteRenderSystem  (systemManager.Entities,
                    textureCollection, spriteBatch)
            });
        }

        /// <summary>
        /// Add the entity prefabs
        /// </summary>
        private void SetupEntityPrefabs()
        {
            foreach (KeyValuePair<string, EntityTemplate> entityPrefab in entityPrefabs)
                systemManager.Entities.AddEntityTemplate(entityPrefab.Key, entityPrefab.Value);
        }

        /// <summary>
        /// Update the game Entities
        /// </summary>
        public override ScreenElement Update(TimeSpan frameStepTime)
        {         
            // Handle the component and entity updates
            systemManager.ProcessComponents(frameStepTime);

            return this;
        }

        /// <summary>
        /// Draw the gameplay screen
        /// </summary>
        public override void Draw(TimeSpan frameStepTime)
        {
            graphicsDevice.SetRenderTarget(screenRT);
            graphicsDevice.Clear(new Color(94, 109, 119));
            systemManager.DrawEntities();
            graphicsDevice.SetRenderTarget(null);

            // Draw render target area to window
            graphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.Default, RasterizerState.CullCounterClockwise);
            spriteBatch.Draw((Texture2D)screenRT, Vector2.Zero, null, Color.White, 0f,
                Vector2.Zero, virtualResolutionRatio, SpriteEffects.None, 0f);
            spriteBatch.End();

            // Draw ECS debug data
            DrawDebugData();

            base.Draw(frameStepTime);
        }

        /// <summary>
        /// Display debug data from ECS
        /// </summary>
        [Conditional("DEBUG")]
        private void DrawDebugData()
        {
            int[] entityInfo = new int[1];
            systemManager.DebugEntities(ref entityInfo);

            spriteBatch.Begin();
            spriteBatch.Draw(pixel, new Rectangle(0, graphicsDevice.Viewport.Height - 88,
                graphicsDevice.Viewport.Width, 108), new Color(0, 0, 0, 0.8f));
            spriteBatch.Draw(pixel, new Rectangle(4 + (200 * 4), graphicsDevice.Viewport.Height - 88, 3, 7), Color.White);

            // Default component colors
            Color[] colors = { new Color(0, 0, 0, 0.5f), Color.White, Color.Blue, Color.Cyan, Color.LightGreen, 
                                 Color.Yellow, Color.Green, Color.Red, Color.Orange, Color.Fuchsia, 
                                 new Color(0, 255, 0), Color.LightSkyBlue };

            // Display component info for each entity
            for (int i = 0; i < entityInfo.Length; i++)
            {
                int j = 1;
                int component = 0;
                while (j < (1 << 10))
                {
                    j = 1 << component;
                    int c = ((int)(entityInfo[i] & j) == j) ? component + 1 : 0;

                    spriteBatch.Draw(pixel, new Rectangle(4 + (i * 4),
                        graphicsDevice.Viewport.Height - 80 + (component * 6), 3, 5), colors[c]);

                    component++;
                }
            }

            spriteBatch.DrawString(debugFont, systemManager.totalEntities.ToString() + " entities",
                new Vector2(2, 2), Color.White);
            spriteBatch.End();
        }
    }
}
