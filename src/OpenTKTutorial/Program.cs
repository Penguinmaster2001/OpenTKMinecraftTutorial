
namespace OpenTKTutorial;

public class Program
{
    static void Main(string[] args)
    {
        using(Game game = new(1440, 900))
        {
            game.Run();
        }
    }
}
