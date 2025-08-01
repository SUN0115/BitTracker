// Services/ICryptoPriceService.cs
using BitTracker.Dtos;

namespace BitTracker.Services
{
    public interface ICryptoPriceService
    {
        /// <summary>
        /// 取即時資料
        /// </summary>
        /// <returns></returns>
        Task<CoinGeckoMarketDto?> GetBitcoinMarketDataAsync();

        /// <summary>
        /// 每日均價列表
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        Task<List<DailyAverageDto>> GetDailyAverageHistoryAsync(int days);
    }
}
