using System.Reflection;
using System.Threading;


public class Program
{


    public static void Task2()
    {
        Object cat1 = new Object();
        Console.WriteLine(GC.GetGeneration(cat1));
        GC.Collect();
        Console.WriteLine(GC.GetGeneration(cat1));
        GC.Collect(); // ну GC принудительно мусор очищает. вот он мне вывел в терминале 0 1 2, потому что GC оставил только те данные, которые еще нужны. ну и вот они поэтапно из одного поколения в другое и переходят
        Console.WriteLine(GC.GetGeneration(cat1));
        GC.Collect();
    }

    // public static void Task3()
    // {
    //     string input =  Console.ReadLine();
    //     int num =  int.Parse(input);
    //     Thread cat2 = new Thread(sum);
    //     cat2.Start(num);
    //     static void Sum
    //     {
    //     int sum = 0;
    //     
    //     for (i = 1; i < num; i++)
    //     {
    //         
    //     }
    // } я клянусь я не знаю
    static void Main()
    {
        Task2();
    }
}