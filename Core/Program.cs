﻿using CommandLine;
using QuantumCore.Auth;
using QuantumCore.Core;
using QuantumCore.Core.Logging;
using QuantumCore.Game;
using Serilog;

namespace QuantumCore
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<AuthOptions, GameOptions>(args).WithParsed(Run);
        }

        private static void Run(object obj)
        {
            Configurator.EnableLogging();

            IServer server = obj switch
            {
                AuthOptions auth => new AuthServer(auth),
                GameOptions game => new GameServer(),
                _ => null
            };

            server?.Start().Wait();
        }
    }
}