namespace PullRequestQuantifier.Client.Helpers
{
    using System;

    public static class QuantifyClientHelper
    {
        public static ConsoleColor GetColor(string color)
        {
            return color switch
            {
                nameof(ConsoleColor.Black) => ConsoleColor.Black,
                nameof(ConsoleColor.DarkBlue) => ConsoleColor.DarkBlue,
                nameof(ConsoleColor.DarkGreen) => ConsoleColor.DarkGreen,
                nameof(ConsoleColor.DarkCyan) => ConsoleColor.DarkCyan,
                nameof(ConsoleColor.DarkRed) => ConsoleColor.DarkRed,
                nameof(ConsoleColor.DarkMagenta) => ConsoleColor.DarkMagenta,
                nameof(ConsoleColor.DarkYellow) => ConsoleColor.DarkYellow,
                nameof(ConsoleColor.Gray) => ConsoleColor.Gray,
                nameof(ConsoleColor.DarkGray) => ConsoleColor.DarkGray,
                nameof(ConsoleColor.Blue) => ConsoleColor.Blue,
                nameof(ConsoleColor.Green) => ConsoleColor.Green,
                nameof(ConsoleColor.Cyan) => ConsoleColor.Cyan,
                nameof(ConsoleColor.Red) => ConsoleColor.Red,
                nameof(ConsoleColor.Magenta) => ConsoleColor.Magenta,
                nameof(ConsoleColor.Yellow) => ConsoleColor.Yellow,
                nameof(ConsoleColor.White) => ConsoleColor.White,
                _ => ConsoleColor.DarkGray,
            };
        }
    }
}
