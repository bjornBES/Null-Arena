/*
 * File: InputSystem.cs
 * File Created: 26 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 27 May 2026
 * Modified By: BjornBEs
 * -----
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PlayerClient.Game.Gameplay.InputSubsystem.ActionInputSystem;
using System.Text.Json;
using System.Text.Json.Serialization;
using MonoButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using MonoKeyboard = Microsoft.Xna.Framework.Input.Keyboard;
using MonoKeys = Microsoft.Xna.Framework.Input.Keys;

namespace PlayerClient.Game.Gameplay.InputSubsystem
{
    public sealed class InputSystem
    {
        internal InputState _state = new InputState();

        private InputProfile _profile;

        private Dictionary<InputActionDefinition, ActionState> _actionStates = new Dictionary<InputActionDefinition, ActionState>();


        internal InputSystem()
        {
            JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };
            serializerOptions.Converters.Add(new JsonStringEnumConverter());

            string json = File.ReadAllText("Content/inputFile.json");
            _profile = JsonSerializer.Deserialize<InputProfile>(json, serializerOptions);

            Update();

            Input._system = this;
        }

        internal void Update()
        {
            _state.PreviousKeyboard = _state.Keyboard;
            _state.PreviousMouse = _state.Mouse;

            _state.Keyboard = MonoKeyboard.GetState();
            _state.Mouse = Mouse.GetState();

            Point center = new Point(WindowManager.Game.ClientBounds.Width / 2, WindowManager.Game.ClientBounds.Height / 2);
            _state.MouseDelta.X = _state.Mouse.X - center.X;
            _state.MouseDelta.Y = _state.Mouse.Y - center.Y;
            Mouse.SetPosition(center.X, center.Y);

            EvaluateActions();
        }

        private void EvaluateActions()
        {
            foreach (InputActionDefinition action in _profile.Actions)
            {
                foreach (InputBinding binding in action.Bindings)
                {
                    if (binding.ReturnType != action.ActionType)
                    {
                        throw new Exception($"Binding return type ({binding.ReturnType}) does not match {action.ActionType})");
                    }
                    ActionState state;
                    if (!_actionStates.TryGetValue(action, out state))
                    {
                        state = new ActionState();
                    }
                    // ActionState rawState = new ActionState();
                    binding.Evaluate(this, ref state);

                    _actionStates[action] = state;
                }
            }
        }

        private InputActionDefinition GetInputAction(string actionName)
        {
            foreach (InputActionDefinition action in _profile.Actions)
            {
                if (action.Action == actionName)
                {
                    return action;
                }
            }
            return null;
        }

        public bool IsActionDown(string action)
            => _actionStates[GetInputAction(action)].BoolDown;
        public bool IsActionPressed(string action)
            => _actionStates[GetInputAction(action)].BoolPressed;
        public bool IsActionReleased(string action)
            => _actionStates[GetInputAction(action)].BoolReleased;

        public float GetAxis(string action)
            => _actionStates[GetInputAction(action)].FloatValue;

        public bool IsKeyDown(KeyCode key)
            => _state.Keyboard.IsKeyDown((MonoKeys)key);

        public bool IsKeyPressed(KeyCode key)
            => _state.Keyboard.IsKeyDown((MonoKeys)key)
            && !_state.PreviousKeyboard.IsKeyDown((MonoKeys)key);

        public bool IsKeyReleased(KeyCode key)
            => !_state.Keyboard.IsKeyDown((MonoKeys)key)
            && _state.PreviousKeyboard.IsKeyDown((MonoKeys)key);

        internal MonoButtonState GetButtonState(MouseButton mouseButton, Microsoft.Xna.Framework.Input.MouseState mouseState)
        {
            switch (mouseButton)
            {
                case MouseButton.Left:
                    return mouseState.LeftButton;
                case MouseButton.Right:
                    return mouseState.RightButton;
                case MouseButton.Middle:
                    return mouseState.MiddleButton;
                case MouseButton.M4:
                    return mouseState.XButton1;
                case MouseButton.M5:
                    return mouseState.XButton2;
            }
            return (MonoButtonState)(-1);
        }

        public bool IsMouseDown(MouseButton mouseButton)
        => GetButtonState(mouseButton, _state.Mouse) == MonoButtonState.Pressed;
        public bool IsMouseUp(MouseButton mouseButton)
        => GetButtonState(mouseButton, _state.Mouse) == MonoButtonState.Released
        && GetButtonState(mouseButton, _state.PreviousMouse) == MonoButtonState.Pressed;
        public bool IsMousePressed(MouseButton mouseButton)
        => GetButtonState(mouseButton, _state.Mouse) == MonoButtonState.Pressed
        && GetButtonState(mouseButton, _state.PreviousMouse) == MonoButtonState.Released;

        public Vector2 MousePosition => MousePosition;
        public Vector2 MousePositionDelta => _state.MouseDelta;


        public float VerticalScrollWheelValue => _state.Mouse.ScrollWheelValue;
        public float VerticalScrollWheelValueDelta => VerticalScrollWheelValue - _state.PreviousMouse.ScrollWheelValue;
        public float HorizontalScrollWheelValue => _state.Mouse.HorizontalScrollWheelValue;
        public float HorizontalScrollWheelValueDelta => HorizontalScrollWheelValue - _state.PreviousMouse.HorizontalScrollWheelValue;

        public bool IsLeftClickPressed()
            => _state.Mouse.LeftButton == (MonoButtonState)ButtonInputState.Pressed
            && _state.PreviousMouse.LeftButton == (MonoButtonState)ButtonInputState.Released;
    }
}
