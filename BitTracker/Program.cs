using BitTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// �w�q�@�Ӧn�O�� CORS �����W��
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddControllers();

// ���U HttpClient �M�ڭ̪�����A��
var coingeckoApiKey = Environment.GetEnvironmentVariable("COINGECKO_API_KEY");
//coingeckoApiKey = "CG-h11111114Mx95BSL2AUFMRc6Q";
coingeckoApiKey = "CG-hS5MRJd4Mx95BSL2AUFMRc6Q";


builder.Services.AddHttpClient<ICryptoPriceService, CoinGeckoPriceService>(client =>
{
    // 1. �ϥ� Pro API �����T���}
    client.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");

    // 2. �N API Key �@�� Header �o�e
    if (!string.IsNullOrEmpty(coingeckoApiKey))
    {
        client.DefaultRequestHeaders.Add("x-cg-demo-api-key", coingeckoApiKey);
    }

    // 3. �u�O�d�@��²�檺 User-Agent
    client.DefaultRequestHeaders.Add("User-Agent", "My-Bitcoin-Tracker-App");

})
// 4. �ҥΦ۰ʸ����Y
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AutomaticDecompression = System.Net.DecompressionMethods.GZip
                           | System.Net.DecompressionMethods.Deflate
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// �[�J CORS �A�ȡA�ó]�w�@�ӵ����Ӥ��\�A�� Vue �e��
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          // ���\�A�� Vue �e�ݨӷ� (�ھڧA�����~�T���O localhost:3000)
                          policy.WithOrigins("http://localhost:3000",
                                            "http://192.168.250.143:3000",
                           "https://btc-tracker-ui-1rjs.vercel.app")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// �b�Ҧ����Ѥ��e�A�ҥΧA�]�w�n�� CORS ����
// **�o�@�檺���ǫD�`���n�I**
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
