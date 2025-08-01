// Dtos/CoinGeckoMarketDto.cs
using System.Text.Json.Serialization;

namespace BitTracker.Dtos
{
    /// <summary>
    /// 現在價格、24_H、24_L
    /// </summary>
    public class CoinGeckoMarketDto
    {
        [JsonPropertyName("current_price")]
        public decimal CurrentPrice { get; set; }

        [JsonPropertyName("high_24h")]
        public decimal High24h { get; set; }

        [JsonPropertyName("low_24h")]
        public decimal Low24h { get; set; }
    }

    /// <summary>
    /// 即時 + 三日均價(包含最大漲跌幅)
    /// </summary>
    public class MainDashboardDto
    {
        // 即時資訊區塊
        public decimal CurrentPrice { get; set; }
        public decimal CurrentPriceUp5Percent { get; set; }
        public decimal CurrentPriceDown5Percent { get; set; }
        public decimal High24h { get; set; }
        public decimal Low24h { get; set; }

        // 三日均價區塊
        public decimal ThreeDayAveragePrice { get; set; }
        public decimal ThreeDayAveragePriceUp5Percent { get; set; }
        public decimal ThreeDayAveragePriceDown5Percent { get; set; }
        public decimal ThreeDayAverageUpRate { get; set; }
        public decimal ThreeDayAverageDownRatet { get; set; }
    }

    /// <summary>
    /// 回傳物件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RespondObj<T>
    {
        public required string ReturnCode { get; set; }
        public required string ReturnMess { get; set; }

        public T? Data { get; set; }
    }

    /// <summary>
    /// 每日均價、最大漲跌幅
    /// </summary>
    public class DailyAverageDto
    {
        public DateOnly Date { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal MaxRate { get; set; }
        public decimal MinRate { get; set; }
    }

}

// { 1, succ}
// {-1, 取得即時資料失敗。取得3天歷史資料失敗}
// {-2, 取得即時資料失敗。取得3天歷史資料失敗}