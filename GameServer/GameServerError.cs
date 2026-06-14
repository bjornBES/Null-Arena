/*
 * File: GameServerError.cs
 * File Created: 13 Jun 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 13 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

namespace GameServer
{
    public static class GameServerError
    {
        public static void DisplayError(string message, string code, Exception exception)
        {
            Console.WriteLine("Error:");
            Console.WriteLine(message);
            Console.WriteLine(code);
            throw exception;
        }
    }
}
