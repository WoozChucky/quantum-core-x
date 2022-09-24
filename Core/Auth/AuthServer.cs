using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuantumCore.API;
using QuantumCore.API.Game.Types;
using QuantumCore.Auth.Cache;
using QuantumCore.Auth.Packets;
using QuantumCore.Cache;
using QuantumCore.Core.Networking;
using QuantumCore.Core.Utils;
using QuantumCore.Database;

namespace QuantumCore.Auth
{
    public class AuthServer : ServerBase<AuthConnection>
    {
        private readonly ILogger<AuthServer> _logger;
        private readonly AuthOptions _options;
        private readonly IDatabaseManager _databaseManager;

        public AuthServer(IOptions<AuthOptions> options, IPacketManager packetManager, ILogger<AuthServer> logger, 
            PluginExecutor pluginExecutor, IServiceProvider serviceProvider, IDatabaseManager databaseManager, IEnumerable<IPacketHandler> packetHandlers)
            : base(packetManager, logger, pluginExecutor, serviceProvider, packetHandlers, options.Value.Port)
        {
            _logger = logger;
            _databaseManager = databaseManager;
            _options = options.Value;
            
            Services.AddSingleton(_ => this);
        }

        protected async override Task ExecuteAsync(CancellationToken token)
        {
            // Initialize static components
            _databaseManager.Init(_options.AccountString, _options.GameString);
            CacheManager.Init(_databaseManager, _options.RedisHost, _options.RedisPort);

            // Register auth server features
            PacketManager.RegisterNamespace("QuantumCore.Auth.Packets");
            RegisterNewConnectionListener(NewConnection);
            
            var pong = await CacheManager.Instance.Ping();
            if (!pong)
            {
                _logger.LogError("Failed to ping redis server");
            }
        }

        private async Task<bool> NewConnection(Connection connection)
        {
            await connection.SetPhase(EPhases.Auth);
            return true;
        }
    }
}