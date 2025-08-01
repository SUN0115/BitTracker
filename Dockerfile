# Dockerfile (最終修正版，完全符合你的結構)

# --- 建置階段 ---
# 使用官方 .NET 8 SDK 映像檔作為建置環境
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. 將 .csproj 檔案從你的 "BitTracker" 子資料夾複製出來
# 語法是：COPY ["來源路徑", "目標路徑"]
COPY ["BitTracker/BitTracker.csproj", "BitTracker/"]

# 2. 執行套件還原，並指定正確的 .csproj 檔案路徑
RUN dotnet restore "BitTracker/BitTracker.csproj"

# 3. 複製所有剩餘的原始碼到 Docker 的 /src 資料夾中
COPY . .

# 4. 指定工作目錄到你的專案資料夾內
WORKDIR "/src/BitTracker"
# 執行發佈，這個指令會編譯你的專案並輸出到 /app/publish
RUN dotnet publish "BitTracker.csproj" -c Release -o /app/publish


# --- 最終運行階段 ---
# 使用輕量的 ASP.NET 執行環境映像檔
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 5. 執行你的 .dll 檔案
ENTRYPOINT ["dotnet", "BitTracker.dll"]