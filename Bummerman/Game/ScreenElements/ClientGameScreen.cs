using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Meteor.ECS;

namespace Bummerman.ScreenElements
{
    /// <summary>
    /// Wrapper for Game Screen that sets up a client to connect to a game server
    /// </summary>
    class ClientGameScreen : GameScreen
    {
        /// <summary>
        /// Setup game client
        /// </summary>
        public ClientGameScreen(Game game, ScreenElement previousScreenElement) :
            base(game, previousScreenElement)
        {
            // Set player ID and add player to the level
            activePlayer = 1;
            level.LoadPlayer(systemManager.Entities, activePlayer);

            // Default message
            networkMessage = "";

            // Add a GameClient System to the ECS
            systemManager.AddSystems(new EntitySystem[] 
            { 
                new Systems.GameClientSystem(systemManager.Entities, spriteBatch, debugFont) 
            });
        }
    }
}
