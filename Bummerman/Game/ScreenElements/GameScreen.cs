using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Lidgren.Network;
using Meteor.ECS;

namespace Bummerman.ScreenElements
{
    partial class GameScreen : DrawableScreenElement
    {
        // ECS entity manager
        protected SystemManager systemManager;

        // Sprite textures and other assets
        protected SpriteFont debugFont;
        Dictionary<string, Texture2D> textureCollection;
        Dictionary<string, Model> modelCollection;
        Dictionary<string, Effect> effectCollection;

        // Game resources
        protected Level level;
        protected int activePlayer = 0;

        // Debugging network status
        protected String networkMessage;

        /// <summary>
        /// Setup ECS framework for game entitise
        /// </summary>
        public GameScreen(Game game, ScreenElement previousScreenElement) : 
            base(previousScreenElement, game.GraphicsDevice)
        {
            // Setup asset collections
            textureCollection = new Dictionary<string, Texture2D>();
            modelCollection = new Dictionary<string, Model>();
            effectCollection = new Dictionary<string, Effect>();

            // Setup game assets
            textureCollection.Add("default", pixel);
            textureCollection.Add("sprites", game.Content.Load<Texture2D>("textures/sprites"));
            textureCollection.Add("blocks", game.Content.Load<Texture2D>("textures/blocks"));
            textureCollection.Add("barrel_dm", game.Content.Load<Texture2D>("textures/barrel_dm"));

            modelCollection.Add("solidBlock1", game.Content.Load<Model>("models/solidBlock1"));
            modelCollection.Add("barrel", game.Content.Load<Model>("models/barrel"));

            // Setup game effects
            effectCollection.Add("default", game.Content.Load<Effect>("effects/default"));
            effectCollection.Add("billboard", game.Content.Load<Effect>("effects/billboard"));

            debugFont = game.Content.Load<SpriteFont>("debug");

            this.game = game;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(graphicsDevice);

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

            systemManager = new SystemManager(new Dictionary<Int32, Component[]>
            { 
                { (int)ComponentType.ScreenPosition, new Components.ScreenPosition[maxEntities]},
                { (int)ComponentType.TilePosition,   new Components.TilePosition[maxEntities]},
                { (int)ComponentType.Sprite,         new Components.Sprite[maxEntities]},
                { (int)ComponentType.MeshModel,      new Components.MeshModel[maxEntities]},
                { (int)ComponentType.Camera,         new Components.Camera[maxEntities]},
                { (int)ComponentType.InputContext,   new Components.InputContext[maxEntities]},
                { (int)ComponentType.Collision,      new Components.Collision[maxEntities]},
                { (int)ComponentType.PlayerInfo,     new Components.PlayerInfo[maxEntities]},
                { (int)ComponentType.Bomb,           new Components.Bomb[maxEntities]},
                { (int)ComponentType.PowerUp,        new Components.PowerUp[maxEntities]},
                { (int)ComponentType.Spreadable,     new Components.Spreadable[maxEntities]},
                { (int)ComponentType.TimedEffect,    new Components.TimedEffect[maxEntities]}
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
                    textureCollection, effectCollection, spriteBatch),
                new ModelRenderSystem   (systemManager.Entities,
                    modelCollection, effectCollection, textureCollection),
                new BillboardSpriteSystem (systemManager.Entities,
                    textureCollection, effectCollection, graphicsDevice),
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
            // Quit the current game
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // Handle the component and entity updates
            systemManager.ProcessComponents(frameStepTime);

            return base.Update(frameStepTime);
        }

        /// <summary>
        /// Draw the gameplay screen
        /// </summary>
        public override void Draw(TimeSpan frameStepTime)
        {
            systemManager.DrawEntities();

            // Debug network data stuff here
            spriteBatch.Begin();
            spriteBatch.DrawString(debugFont, networkMessage, new Vector2(2, 2), Color.White);
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
                new Vector2(2, 22), Color.White);
            spriteBatch.End();
        }
    }
}
