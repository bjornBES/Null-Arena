/*
 * File: InputState.cs
 * File Created: 26 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 27 May 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PlayerClient.Game.Gameplay.InputSubsystem
{
    public sealed class InputState
    {
        public KeyboardState Keyboard;
        public KeyboardState PreviousKeyboard;

        public MouseState Mouse;
        public MouseState PreviousMouse;

        public Vector2 MouseDelta;
    }

}
