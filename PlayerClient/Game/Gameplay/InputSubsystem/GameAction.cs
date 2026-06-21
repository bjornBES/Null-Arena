/*
 * File: GameAction.cs
 * File Created: 26 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 21 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

namespace PlayerClient.Game.Gameplay.InputSubsystem
{
    public class GameAction
    {
        public static InputActionFlags IsInputDown(InputActionFlags actionFlag, Func<string, bool> actionDownFunc, Action action)
        {
            string actionFlagName = actionFlag.ToString();
            if (actionDownFunc(actionFlagName))
            {
                actionDownFunc(actionFlagName);
                action();
                return actionFlag;
            }
            return 0;
        }
    }
}