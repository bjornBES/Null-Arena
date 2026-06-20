/*
 * File: KeyInput.cs
 * File Created: 26 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 27 May 2026
 * Modified By: BjornBEs
 * -----
 */

namespace Shared.Game.InputSystem
{
    [Flags]
    public enum InputActionFlags : uint
    {
        Right = 1 << 0,
        Left = 1 << 1,
        Forward = 1 << 2,
        Backward = 1 << 3,
        Fire1 = 1 << 4,
        Fire2 = 1 << 5,
        Reload = 1 << 6,
        Use = 1 << 7,
        Jump = 1 << 8,
        Crouch = 1 << 9,
        Ability1 = 1 << 10,
        Ability2 = 1 << 11,
        Ability3 = 1 << 12,
        Ability4 = 1 << 13,
        Ability5 = 1 << 14,
        Ability6 = 1 << 15,
        Ability7 = 1 << 16,
        Debug = (uint)1 << 31,
    }
}
