# Dockerfile (修正版)

# --- 建置階段 ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. 只複製 .csproj 檔案到對應的子資料夾結構中
#    請將下面的 "BtcTracker.LightweightApi" 換成你真實的專案資料夾名稱
COPY ["BitTracker/BitTracker.csproj", "BitTracker/"]

# 2. 執行套件還原
RUN dotnet restore "BitTracker./BitTracker.csproj"

# 3. 複製所有剩餘的原始碼
COPY . .

# 4. 在專案資料夾內執行建置與發佈
WORKDIR "/src/BitTracker"
RUN dotnet publish "BitTracker.csproj" -c Release -o /app/publish

# --- 最終運行階段 ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 5. 執行 .dll 檔案
#    同樣，請確認 .dll 檔案名稱與你的專案名稱一致
ENTRYPOINT ["dotnet", "BitTracker.dll"]