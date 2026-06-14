using Microsoft.Xna.Framework;
using PlayerClient.Game.Content;
using PlayerClient.Game.Gameplay.NetworkSystem;
using Shared.Game.Maps;
using Shared.Network.Package.matchs;

namespace PlayerClient.Game.Gameplay.MapSystem
{
    public class MapManager : ManagerClass
    {
        public override void Initialize(GameplayContext context)
        {
            Task.Run(MapRegistry.Init);

            PacketDespasher.OnMatchMapPackages += GotMatchMap;
        }
        public override void LoadContent(GameplayContext context, EngineContentManager contentManager)
        {
        }

        public override void StartMatch(GameplayContext context)
        {
        }
        public override void Update(GameplayContext context, GameTime gameTime)
        {
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
        private async void GotMatchMap(MatchMapPackages mapPackage)
        {
            MapParser.GetMap(new string(mapPackage.MapId, 0, mapPackage.MapIdLength));
        }
    }
}
