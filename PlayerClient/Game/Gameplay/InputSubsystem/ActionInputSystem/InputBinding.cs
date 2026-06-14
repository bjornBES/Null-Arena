/*
 * File: InputBinding.cs
 * File Created: 26 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 25 May 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework;
using System.Text.Json.Serialization;

namespace PlayerClient.Game.Gameplay.InputSubsystem.ActionInputSystem
{
    [JsonDerivedType(typeof(FloatAxisBinding), "FloatBinding")]
    [JsonDerivedType(typeof(BoolKeyBinding), "BoolBinding")]
    public abstract class InputBinding
    {
        [JsonIgnore]
        public abstract ActionType ReturnType { get; }

        public InputDevice Device { get; set; }

        [JsonIgnore]
        public abstract BindingType BindingType { get; }

        public abstract void Evaluate(InputSystem system, ref ActionState state);
    }

    public class FloatAxisBinding : InputBinding
    {
        public override BindingType BindingType => BindingType.Axis;
        public override ActionType ReturnType => ActionType.Float;
        public KeyCode[] Positive { get; set; }
        public KeyCode[] Negative { get; set; }

        public override void Evaluate(InputSystem system, ref ActionState state)
        {
            float value = state.FloatValue;

            int valueChanged = 0;
            foreach (KeyCode key in Positive)
            {
                if (system.IsKeyDown(key))
                {
                    value += 0.05f;
                    valueChanged++;
                }
            }

            foreach (KeyCode key in Negative)
            {
                if (system.IsKeyDown(key))
                {
                    value -= 0.05f;
                    valueChanged++;
                }
            }

            value = (float)MathHelper.Clamp(value, -1, 1);

            if (valueChanged == 0)
            {
                float sign = -(float)value;
                value += sign * 0.05f;
            }


            state.FloatValue = value;
        }
    }
    public class BoolKeyBinding : InputBinding
    {
        public override BindingType BindingType => BindingType.Button;
        public override ActionType ReturnType => ActionType.Bool;
        public KeyCode[] Keys { get; set; }

        public override void Evaluate(InputSystem system, ref ActionState state)
        {
            bool down = false;
            bool pressed = false;
            bool released = false;

            foreach (KeyCode key in Keys)
            {
                if (system.IsKeyDown(key))
                {
                    down = true;
                }

                if (system.IsKeyPressed(key))
                {
                    pressed = true;
                }

                if (system.IsKeyReleased(key))
                {
                    released = true;
                }
            }

            state.BoolDown = down;
            state.BoolPressed = pressed;
            state.BoolReleased = released;
        }
    }
}
