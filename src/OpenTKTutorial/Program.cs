
namespace OpenTKTutorial;

public class Program
{
    static void Main(string[] args)
    {
        using(Game game = new(1920, 1080))
        {
            game.Run();
        }
    }
}
