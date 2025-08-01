using BitTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// �w�q�@�Ӧn�O�� CORS �����W��
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddControllers();

// ���U HttpClient �M�ڭ̪�����A��
builder.Services.AddHttpClient<ICryptoPriceService, CoinGeckoPriceService>(client =>
{
    client.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");

    // ������ �����???��?? ������
    client.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
    client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");
    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36");
    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
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
