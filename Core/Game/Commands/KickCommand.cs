﻿using System;
using System.Collections.Generic;
using System.Text;
using QuantumCore.API.Game;
using QuantumCore.API.Game.World;
using QuantumCore.Game.World.Entities;

namespace QuantumCore.Game.Commands
{
    [Command("kick", "Kick a player from the Server")]
    public static class KickCommand
    {
        [CommandMethod]
        public static async void Kick(IPlayerEntity player, IPlayerEntity target)
        {
            target.Disconnect();
        }
    }
}
