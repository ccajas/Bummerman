using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Meteor.ECS;

namespace Bummerman.ScreenElements
{
    /// <summary>
    /// Wrapper for Game Screen that sets up a server to host a game
    /// </summary>
    class ServerGameScreen : GameScreen
    {
        /// <summary>
        /// Setup game server
        /// </summary>
        public ServerGameScreen(Game game, ScreenElement previousScreenElement) :
            base(game, previousScreenElement)
        {
            // Host player is always 0
            activePlayer = 0;

            // Default message
            networkMessage = "";

            // Add a GameServer System to the ECS
            systemManager.AddSystems(new EntitySystem[] 
            { 
                new Systems.GameServerSystem(systemManager.Entities, spriteBatch, debugFont) 
            });
        }
    }
}
