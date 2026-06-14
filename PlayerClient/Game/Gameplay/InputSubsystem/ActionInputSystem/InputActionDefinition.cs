using System.Text.Json.Serialization;

namespace PlayerClient.Game.Gameplay.InputSubsystem.ActionInputSystem
{
    public class InputActionDefinition
    {
        public string Action { get; set; }
        public ActionType ActionType { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public InputBinding[] Bindings { get; set; } = null;
    }
}
