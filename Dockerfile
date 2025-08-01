# Dockerfile
# 使用官方 .NET 8 SDK 映像檔作為建置環境
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 複製 .csproj 檔案並還原 NuGet 套件
COPY *.csproj .
RUN dotnet restore

# 複製所有剩餘的檔案並建置發佈
COPY . .
RUN dotnet publish -c Release -o /app/publish

# 使用輕量的 ASP.NET 執行環境映像檔作為最終的運行環境
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 執行應用程式
# 請將 BtcTracker.LightweightApi.dll 換成你專案的 .dll 檔案名稱
ENTRYPOINT ["dotnet", "BtcTracker.LightweightApi.dll"]