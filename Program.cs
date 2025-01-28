public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<MongoDBContext>();

        builder.Services.AddControllers();

        var app = builder.Build();

        // Test MongoDB connection
        var mongoContext = app.Services.GetRequiredService<MongoDBContext>();
        var isConnected = await mongoContext.TestConnection();
        if (isConnected)
        {
            Console.WriteLine("Successfully connected to MongoDB!");
        }
        else
        {
            Console.WriteLine("Failed to connect to MongoDB. Please check your connection string and ensure MongoDB is running.");
        }

        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
