using BitTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 定義一個好記的 CORS 策略名稱
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddControllers();

// 註冊 HttpClient 和我們的價格服務
builder.Services.AddHttpClient<ICryptoPriceService, CoinGeckoPriceService>(client =>
{
    client.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");

    // ▼▼▼ 完整模???器?? ▼▼▼
    client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
    client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36");
    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
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
