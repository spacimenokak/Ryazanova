using System.Reflection;
using System.Threading;

public class Cat
{
    public string Name { get; set; }
}

public class Program
{
    public static void Task1()
    {
        Cat cat = new Cat();
        Type type = cat.GetType();
        Console.WriteLine(type.Name);
        foreach (PropertyInfo prop in type.GetProperties())
        {
            Console.WriteLine(prop.Name, prop.PropertyType);
        }
        foreach (MethodInfo method in type.GetMethods())
        {
            Console.WriteLine(method.Name, method.ReturnType);
        }
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
        Task1();
    }
}