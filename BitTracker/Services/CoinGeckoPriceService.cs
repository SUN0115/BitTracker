// Services/CoinGeckoPriceService.cs
using BitTracker.Dtos;
using System.Text.Json;

namespace BitTracker.Services
{
    public class CoinGeckoPriceService : ICryptoPriceService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CoinGeckoPriceService> _logger;
        private readonly string? _apiKey; // 宣告一個欄位來儲存金鑰

        public CoinGeckoPriceService(HttpClient httpClient, ILogger<CoinGeckoPriceService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = Environment.GetEnvironmentVariable("COINGECKO_API_KEY");
            _apiKey = "CG-hS5MRJd4Mx95BSL2AUFMRc6Q";
        }

        /// <summary>
        /// 發送 api 取即時價格、24_H、24_L
        /// </summary>
        /// <returns></returns>
        public async Task<CoinGeckoMarketDto?> GetBitcoinMarketDataAsync()
        {
            try
            {
                // 設定請求子網址
                //const string requestUri = "coins/markets?vs_currency=usd&ids=bitcoin";
                var requestUri = $"coins/markets?vs_currency=usd&ids=bitcoin&x_cg_pro_api_key={_apiKey}";

                // 抓出值 並轉成List<CoinGeckoMarketDto>
                var response = await _httpClient.GetFromJsonAsync<List<CoinGeckoMarketDto>>(requestUri);
                // 取第一個
                return response?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"抓取現在價格時發生錯誤 => {ex.Message}");
                //throw new Exception($"抓取現在價格時發生錯誤 => {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 發送 api，根據 days 取每一小時一筆紀錄，並計算日均價 
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public async Task<List<DailyAverageDto>> GetDailyAverageHistoryAsync(int days)
        {
            try
            {
                //var requestUri = $"coins/bitcoin/market_chart?vs_currency=usd&days={days}";
                // 在請求的 URL 中附加上 API Key
                var requestUri = $"coins/bitcoin/market_chart?vs_currency=usd&days={days}&x_cg_pro_api_key={_apiKey}";
                
                // 發送 GET 請求取得資料
                var response = await _httpClient.GetAsync(requestUri);
                // 非2XX 丟出例外
                response.EnsureSuccessStatusCode();

                // 取得回傳內容的資料串流
                using var contentStream = await response.Content.ReadAsStreamAsync();
                // 將 JSON 串流解析成 JsonDocument
                var jsonDoc = await JsonDocument.ParseAsync(contentStream);
                // 取得 JSON 中 "prices" 陣列，開始列舉每筆價格資料
                var prices = jsonDoc.RootElement.GetProperty("prices").EnumerateArray();

                #region 創DailyAverageDto 且分組
                var dailyAverages = prices
                    .Select(p => new // 將每筆 JSON 陣列轉成匿名物件：包含日期與價格
                    {
                        Date = DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeMilliseconds(p[0].GetInt64()).UtcDateTime),
                        Price = p[1].GetDecimal()
                    })
                    .GroupBy(p => p.Date) // 依據日期分組（一天可能有多筆）
                    .Select(group =>
                    {
                        var avg = group.Average(p => p.Price); // 平均價
                        var max = group.Max(p => p.Price);     // 當日最高價
                        var min = group.Min(p => p.Price);     // 當日最低價

                        return new DailyAverageDto
                        {
                            Date = group.Key,                 // 日期
                            AveragePrice = avg,               // 平均價
                            MaxRate = ((max - avg) / avg) * 100 , // 最大漲幅（漲 12%）
                            MinRate = ((min - avg) / avg) * 100  // 最大跌幅（跌 8%）
                        };
                    })
                    .OrderByDescending(g => g.Date) // 依照日期從新到舊排序
                    .ToList();
                #endregion

                return dailyAverages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "發生錯誤：抓取並計算歷史均價失敗 (天數: {Days})。", days);
                return []; // 回傳一個空列表
            }
        }
    }
}
