namespace simpleApi.Basic;

public class BasicConfiguration
{
    private static IConfiguration _configuration;

    static BasicConfiguration()
    {
        // Membaca nilai ISENV dari appsettings.json
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        _configuration = builder.Build();

        // Menggunakan nilai ISENV dari appsettings.json atau default false jika tidak ditemukan
        var isEnvEnabled = _configuration.GetValue("ISENV", false);

        // Menggunakan nilai ISENV untuk memutuskan apakah membaca dari .env atau tidak
        if (isEnvEnabled) DotNetEnv.Env.Load();
    }

    // Mengambil nilai variabel dari .env atau appsettings.json
    public string GetVariable(string variableName)
    {
        string result;
        // Menggunakan nilai ISENV untuk memutuskan dari mana membaca variabel
        if (_configuration.GetValue<bool>("ISENV"))
            return Environment.GetEnvironmentVariable(variableName);
        else
            return _configuration.GetValue<string>(variableName);
    }

    public static string GetVariableGlobal(string variableName)
    {
        string result;
        // Menggunakan nilai ISENV untuk memutuskan dari mana membaca variabel
        if (_configuration.GetValue<bool>("ISENV"))
            return Environment.GetEnvironmentVariable(variableName);
        else
            return _configuration.GetValue<string>(variableName);
    }
}