using example.Core;

internal class Program
{
    /// <summary>
    /// The main entry point for the application. 
    /// This creates an instance of your game and calls the Run() method 
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    private static void Main(string[] args)
    {
        System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
        using var game = new exampleGame();
        game.Run();
    }
}