namespace CampingAI.WebApi;
public class Program {
    public static void Main(string[] args) {
        var app = Startup.Init(args);
        app.Run();




    }
}