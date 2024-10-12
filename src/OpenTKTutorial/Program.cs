
namespace OpenTKTutorial;

public class Program
{
    static void Main(string[] args)
    {
        using(Game game = new(1024, 1024))
        {
            game.Run();
        }
    }
}
