using BitTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 定義一個好記的 CORS 策略名稱
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddControllers();

// 註冊 HttpClient 和我們的價格服務
var coingeckoApiKey = Environment.GetEnvironmentVariable("COINGECKO_API_KEY");
//coingeckoApiKey = "CG-h11111114Mx95BSL2AUFMRc6Q";
coingeckoApiKey = "CG-hS5MRJd4Mx95BSL2AUFMRc6Q";


builder.Services.AddHttpClient<ICryptoPriceService, CoinGeckoPriceService>(client =>
{
    // 1. 使用 Pro API 的正確網址
    client.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");

    // 2. 將 API Key 作為 Header 發送
    if (!string.IsNullOrEmpty(coingeckoApiKey))
    {
        client.DefaultRequestHeaders.Add("x-cg-demo-api-key", coingeckoApiKey);
    }

    // 3. 只保留一個簡單的 User-Agent
    client.DefaultRequestHeaders.Add("User-Agent", "My-Bitcoin-Tracker-App");

})
// 4. 啟用自動解壓縮
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AutomaticDecompression = System.Net.DecompressionMethods.GZip
                           | System.Net.DecompressionMethods.Deflate
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 加入 CORS 服務，並設定一個策略來允許你的 Vue 前端
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          // 允許你的 Vue 前端來源 (根據你的錯誤訊息是 localhost:3000)
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

// 在所有路由之前，啟用你設定好的 CORS 策略
// **這一行的順序非常重要！**
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
