/*
 * File: InputPackage.cs
 * File Created: 26 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 21 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework;
using PlayerClient.Game.Gameplay.InputSubsystem;
using Shared.Ncode.Packages;

namespace PlayerClient.Game.Gameplay.NetworkSystem.Packets
{
    public class InputPackage : Packet
    {
        public override uint Type => (uint)PackageType.Input;

        public Vector3 RawMovement { get; set; }
        public InputActionFlags Buttons { get; set; }
        public float AimX { get; set; }
        public float AimY { get; set; }

        public override void Deserialize(BinaryReader reader)
        {
            Vector3 point = new Vector3();
            point.X = reader.ReadSingle();
            point.Y = reader.ReadSingle();
            RawMovement = point;
            Buttons = (InputActionFlags)reader.ReadUInt32();
            AimX = reader.ReadSingle();
            AimY = reader.ReadSingle();
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(RawMovement.X);
            writer.Write(RawMovement.Y);
            writer.Write((uint)Buttons);
            writer.Write(AimX);
            writer.Write(AimY);
        }
    }
}
