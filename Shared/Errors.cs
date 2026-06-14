/*
 * File: Errors.cs
 * File Created: 13 Jun 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 13 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

namespace Shared
{
    public static class SharedErrors
    {
        public static string FormatErrorMessage(string message, string code)
        {
            return $"{message}{Environment.NewLine}error: {code}";
        }
    }
}
