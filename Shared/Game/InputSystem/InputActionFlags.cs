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
        Fire1 = 1 << 0,
        Fire2 = 1 << 1,
        Reload = 1 << 2,
        Use = 1 << 3,
        Jump = 1 << 4,
        Crouch = 1 << 5,
        Ability1 = 1 << 6,
        Ability2 = 1 << 7,
        Ability3 = 1 << 8,
        Ability4 = 1 << 9,
        Ability5 = 1 << 10,
        Ability6 = 1 << 11,
        Ability7 = 1 << 12,
    }
}