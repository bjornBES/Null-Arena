using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Game.Player
{
    public record PlayerId(byte[] Id);
    public class PlayerIdentity
    {
        public PlayerId PlayerId { get; set; }
        public string PlayerName { get; set; }

        public static PlayerIdentity Generate(string playerName)
        {
            List<byte> bytes =
            [
                .. BitConverter.GetBytes(DateTime.UtcNow.Ticks),
                .. BitConverter.GetBytes(DateTime.UtcNow.Nanosecond),
                .. BitConverter.GetBytes(Mouse.GetState().Position.GetHashCode()),
                .. BitConverter.GetBytes(Mouse.GetState().ScrollWheelValue),
                .. BitConverter.GetBytes(Environment.MachineName.GetHashCode()),
                .. BitConverter.GetBytes(Environment.ProcessId),
                .. BitConverter.GetBytes(Environment.TickCount64),
                .. BitConverter.GetBytes(GraphicsAdapter.Adapters.Count),
                .. BitConverter.GetBytes(GraphicsAdapter.Adapters.GetHashCode()),
                .. Guid.NewGuid().ToByteArray(),
            ];

            SHA256 sha = SHA256.Create();
            byte[] shaBytes = sha.ComputeHash(bytes.ToArray(), 0, bytes.Count);

            if (shaBytes.All(b => b == 0))
            {
                Mouse.SetPosition(20, 30);
                return Generate(playerName);
            }

            return new PlayerIdentity() { PlayerId = new PlayerId(shaBytes), PlayerName = playerName };
        }
    }
}
