public class Program
{
    public static void Main(string[] args)
    {
        var repo = new AttractiesAccess();
        var service = new AttractieLogic(repo);
        AttractieMenu.Start(service);
    }
}