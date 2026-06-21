/*
 * File: InputBuffer.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 21 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework;
using PlayerClient.Game.Gameplay.InputSubsystem;
using PlayerClient.Game.Gameplay.NetworkSystem.Packets;

namespace PlayerClient.Game.Gameplay.EntitySystem
{
    public class InputBuffer
    {
        private Dictionary<ulong, InputPackage> _history = new();
        private ulong _nextSequence = 0;

        public InputPackage Record(Vector3 movement, InputActionFlags buttons, Vector2 mouseMovement)
        {
            InputPackage input = new InputPackage
            {
                Sequence = _nextSequence++,
                RawMovement = movement,
                Buttons = buttons,
                AimX = mouseMovement.X,
                AimY = mouseMovement.Y
            };
            _history[input.Sequence] = input;
            return input;
        }

        public List<InputPackage> GetSince(ulong sequence)
        {
            return _history.Where(kv => kv.Key > sequence)
                            .OrderBy(kv => kv.Key)
                            .Select(kv => kv.Value)
                            .ToList();
        }

        public void Prune(ulong acknowledgedSequence)
        {
            List<ulong> toRemove = _history.Keys.Where(k => k <= acknowledgedSequence).ToList();
            foreach (ulong k in toRemove)
            {
                _history.Remove(k);
            }
        }
    }
}
