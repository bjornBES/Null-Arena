namespace PlayerClient.Game.Gameplay.InputSubsystem.ActionInputSystem
{
    public sealed class InputProfile
    {
        public string ProfileName { get; set; }
        public InputDevice[] InputDevices { get; set; }
        public InputActionDefinition[] Actions { get; set; }
    }
}
