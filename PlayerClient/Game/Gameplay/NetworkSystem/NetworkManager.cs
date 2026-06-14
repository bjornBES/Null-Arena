/*
 * File: NetworkManager.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 25 May 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework;
using PlayerClient.Client.game;
using PlayerClient.Game.Content;
using Shared.Game.Matchs;
using Shared.Network;
using Shared.Network.Package;
using Shared.Network.Package.matchs;

namespace PlayerClient.Game.Gameplay.NetworkSystem
{
    public class NetworkManager : ManagerClass
    {
#nullable enable
        GameClient _gameClient;
        private Queue<Packet> _packetQueue;
        private Queue<Packet> _sendPacketQueue;

        // event
        public event Action<MatchMapPackages>? OnGotMatchMap;
        public NetworkManager() : base()
        {
            _gameClient = new GameClient();
            _packetQueue = new Queue<Packet>();
            _sendPacketQueue = new Queue<Packet>();
        }

        public override void Initialize(GameplayContext context)
        {
            _gameClient.Initialize();
            _packetQueue = new Queue<Packet>();
            _sendPacketQueue = new Queue<Packet>();
        }
        public override void LoadContent(GameplayContext context, EngineContentManager contentManager)
        {
        }

        public override void StartMatch(GameplayContext context)
        {
        }
        public override void Update(GameplayContext context, GameTime gameTime)
        {
            while (_packetQueue.Count > 0)
            {
                Packet packet = _packetQueue.Dequeue();
                switch (packet.Type)
                {
                    case PackageType.Input:
                        break;
                    case PackageType.FindMatch:
                        break;
                    case PackageType.ConnectMatch:
                        break;
                    case PackageType.GetMatchMap:
                        break;
                    default:
                        break;
                }
            }
        }
        public override void PostPhysicsUpdate(GameplayContext context, GameTime gameTime)
        {
        }
        public override void Draw(GameplayContext context, GameTime gameTime)
        {
        }
        public override void Unload(GameplayContext context)
        {
        }

        public void QueueRecvPackage(Packet packet)
        {
            _packetQueue.Enqueue(packet);
        }

        public void QueueSendPackage(Packet packet)
        {
            _sendPacketQueue.Enqueue(packet);
        }

        public uint ConnectToServer(ServerInfo server)
        {
            return _gameClient.Connect(server, QueueRecvPackage);
        }
        public void ConnectToMatch(MatchId id)
        {
            Packet packet = new MatchMapPackages();
            _gameClient.SendPackage(packet);
        }
    }
}
