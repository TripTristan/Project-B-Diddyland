public class Program
{
    static void Main()
    {
        var access = new MenusAccess();
        var logic = new MenuLogic(access);

        // Start the console UI
        MenuForm.Run(logic);
    }
}

