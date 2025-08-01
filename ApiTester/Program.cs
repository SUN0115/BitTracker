// Program.cs of ApiTester (v2 - 必定讀取回應內容版)

using System.Net.Http.Headers;
using System.Net;

// --- 設定區 ---
const string ApiKey = "CG-hS5MRJd4Mx95BSL2AUFMRc6Q";
const string RequestUrl = "https://api.coingecko.com/api/v3/coins/markets?vs_currency=usd&ids=bitcoin";

Console.WriteLine("正在執行 API 請求測試 (v2)...");

using (var client = new HttpClient())
{
    // 我們只加入 API Key，保持最簡潔的請求
    client.DefaultRequestHeaders.Add("x-cg-demo-api-key", ApiKey);

    try
    {
        Console.WriteLine($"發送到: {RequestUrl}");

        // 發送請求
        var response = await client.GetAsync(RequestUrl);

        // **關鍵**：無論成功或失敗，都先讀取回應內容
        string responseBody = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"\n回應狀態碼: {(int)response.StatusCode} ({response.ReasonPhrase})");
        Console.WriteLine("------ 伺服器原始回應內容 ------");
        Console.WriteLine(responseBody);
        Console.WriteLine("----------------------------------");

        // 如果請求不成功，手動拋出例外，方便觀察
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"請求失敗，狀態碼: {response.StatusCode}");
        }

        Console.WriteLine("\n請求成功！");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n------ 測試中發生例外 ------");
        Console.WriteLine(ex.Message);
    }
}