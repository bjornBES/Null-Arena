internal class Program
{
    private static void Main(string[] args)
    {
        using var game = new PlayerClient.Game1();
        game.Run();
    }
}