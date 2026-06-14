/*
 * File: Input.cs
 * File Created: 26 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 27 May 2026
 * Modified By: BjornBEs
 * -----
 */


using Microsoft.Xna.Framework;


namespace PlayerClient.Game.Gameplay.InputSubsystem
{
    public static class Input
    {
        internal static InputSystem _system;
        static InputState _state => _system._state;

        // -------- Keyboard --------

        public static bool IsKeyDown(KeyCode key) => _system.IsKeyDown(key);
        public static bool IsKeyPressed(KeyCode key) => _system.IsKeyPressed(key);
        public static bool IsKeyReleased(KeyCode key) => _system.IsKeyReleased(key);

        // -------- Mouse --------

        public static Vector2 MousePosition => _system.MousePosition;

        public static Vector2 MousePositionDelta => _system.MousePositionDelta;
        public static float MouseScrollDelta => _system.VerticalScrollWheelValueDelta;

        public static bool IsLeftClickPressed() => _system.IsMousePressed(MouseButton.Left);

        // Actions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsActionDown(string key) => _system.IsActionDown(key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsActionPressed(string key) => _system.IsActionPressed(key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsActionReleased(string key) => _system.IsActionReleased(key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static float GetAxis(string key) => _system.GetAxis(key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static float GetAxisRaw(string key)
        {
            float valueRaw = _system.GetAxis(key);
            float value = (float)Math.Round(valueRaw, 1, MidpointRounding.AwayFromZero);
            if (value != 0)
            {
                Console.WriteLine($"GetAxisRaw({key}) = {value}");
            }
            return value;
        }
    }
}