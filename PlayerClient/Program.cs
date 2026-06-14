using PlayerClient;

internal class Program
{
    private static void Main(string[] args)
    {
        using Game1 game = new Game1(args);
        game.Run();
    }
}
