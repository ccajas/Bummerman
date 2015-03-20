using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Network;
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
            // Default message
            networkMessage = "Setting up server...";

            // Add a GameServer System to the ECS
            systemManager.AddSystems(new EntitySystem[] 
            { 
                new Systems.GameServerSystem(systemManager.Entities) 
            });
        }
    }
}
