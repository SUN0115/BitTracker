// Controllers/DashboardController.cs
using BitTracker.Dtos;
using BitTracker.Services;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BitTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        // 服務實例
        private readonly ICryptoPriceService _priceService;

        public DashboardController(ICryptoPriceService priceService)
        {
            _priceService = priceService;
        }

        /// <summary>
        /// 查即時資訊、歷史三天資訊
        /// </summary>
        /// <returns></returns>
        [HttpGet("main")] // GET /api/dashboard/main
        public async Task<ActionResult<RespondObj<MainDashboardDto>>> GetMainDashboard()
        {
            var marketDataTask = _priceService.GetBitcoinMarketDataAsync();
            var dailyAveragesTask = _priceService.GetDailyAverageHistoryAsync(3);

            await Task.WhenAll(marketDataTask, dailyAveragesTask);

            var marketData = await marketDataTask;
            var dailyAverages = await dailyAveragesTask;

            var respondObj = new RespondObj<MainDashboardDto>
            {
                ReturnCode = "",
                ReturnMess = "",
                Data = null
            }; // 回傳物件

            if (marketData == null || dailyAverages.Count < 3)
            {
                respondObj.ReturnCode = "-1";
                if (marketData == null) {
                    respondObj.ReturnMess = "取得即時資料失敗。";
                }
                if(dailyAverages.Count < 3)
                {
                    respondObj.ReturnMess += "取得3天歷史資料失敗。";
                }
                return StatusCode(503, respondObj);
            } // 資料有誤

            // 計算平均值
            var threeDayAveragePrice = dailyAverages.Take(3).Average(d => d.AveragePrice);
            var threeDayUpRate = dailyAverages.Take(3).Average(d => d.MaxRate);
            var threeDayDownRate = dailyAverages.Take(3).Average(d => d.MinRate);

            #region 組裝正確回傳物件
            var mainDashboardDto = new MainDashboardDto
            {
                CurrentPrice = marketData.CurrentPrice,
                CurrentPriceUp5Percent = Math.Round(marketData.CurrentPrice * 1.0025m, 2),
                CurrentPriceDown5Percent = Math.Round(marketData.CurrentPrice * 0.9975m, 2),
                High24h = marketData.High24h,
                Low24h = marketData.Low24h, 
                ThreeDayAveragePrice = Math.Round(threeDayAveragePrice, 2),
                ThreeDayAveragePriceUp5Percent = Math.Round(threeDayAveragePrice * 1.0025m, 2),
                ThreeDayAveragePriceDown5Percent = Math.Round(threeDayAveragePrice * 0.9975m, 2),
                ThreeDayAverageUpRate = Math.Round(threeDayUpRate, 2),
                ThreeDayAverageDownRatet = Math.Round(threeDayDownRate, 2),
            };
            respondObj.ReturnCode = "1";
            respondObj.ReturnMess = "succ";
            respondObj.Data= mainDashboardDto;
            #endregion

            return Ok(respondObj);
        }

        /// <summary>
        /// 查歷史90天平均價格
        /// </summary>
        /// <returns></returns>
        [HttpGet("historical-chart")] // GET /api/dashboard/historical-chart
        public async Task<ActionResult<RespondObj<List<DailyAverageDto>>>> GetHistoricalChart()
        {
            // 獲取計算好的 90 天每日均價列表
            var dailyAverages = await _priceService.GetDailyAverageHistoryAsync(90);

            var respondObj = new RespondObj<List<DailyAverageDto>>
            {
                ReturnCode = "",
                ReturnMess = "",
                Data = null
            }; // 創回傳物件

            if (!dailyAverages.Any())
            {
                respondObj.ReturnCode = "-2";
                respondObj.ReturnMess = "取得90天歷史資料失敗";
                return StatusCode(503, respondObj);
            } // 錯誤回傳

            #region 組裝回傳 Data
            // 資料轉換成 DailyAverageDto 格式
            var chartData = dailyAverages
                  .OrderBy(d => d.Date) // 確保圖表數據是按時間升序排列
                  .Select(d => new DailyAverageDto // 為每個項目建立一個新的 DailyAverageDto 物件
                  {
                      Date = d.Date,
                      AveragePrice = Math.Round(d.AveragePrice, 2),
                      MaxRate = Math.Round(d.MaxRate, 2),
                      MinRate = Math.Round(d.MinRate, 2),
                  })
                  .ToList(); // 將結果轉換成 List
            #endregion

            respondObj.ReturnCode = "1";
            respondObj.ReturnMess = "succ";
            respondObj.Data = chartData;

            return Ok(respondObj);
        }
    }
}
