namespace RedisShop.Utils
{
    class Logger
    {
        public static void Log(string sender, LogType type, string message)
        {
            // Choose log color
            Console.ForegroundColor = type switch
            {
                LogType.INFO => ConsoleColor.White,
                LogType.WARRNING => ConsoleColor.DarkYellow,
                LogType.ERROR => ConsoleColor.Red,
                LogType.SUCCESS => ConsoleColor.Green,
                _ => ConsoleColor.White
            };

            // Construct and print log
            string log = $"[{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")}] {type,-9}:{sender,10} | {message}";
            Console.WriteLine(log);

            // Reset console color
            Console.ResetColor();

            return;
        }
    }
}
