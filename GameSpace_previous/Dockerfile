# 使用官方 .NET 8.0 運行時映像作為基礎映像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# 使用官方 .NET 8.0 SDK 映像進行構建
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 複製項目文件
COPY ["GameSpace/GameSpace.csproj", "GameSpace/"]
COPY ["GameSpace.Infrastructure/GameSpace.Infrastructure.csproj", "GameSpace.Infrastructure/"]
RUN dotnet restore "GameSpace/GameSpace.csproj"

# 複製所有源代碼
COPY . .
WORKDIR "/src/GameSpace"

# 構建應用程序
RUN dotnet build "GameSpace.csproj" -c Release -o /app/build

# 發布應用程序
FROM build AS publish
RUN dotnet publish "GameSpace.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 創建最終運行時映像
FROM base AS final
WORKDIR /app

# 安裝必要的系統包
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

# 複製發布的應用程序
COPY --from=publish /app/publish .

# 創建日誌目錄
RUN mkdir -p /app/logs

# 設置權限
RUN chmod 755 /app

# 創建非 root 用戶
RUN adduser --disabled-password --gecos '' gamespace
RUN chown -R gamespace:gamespace /app
USER gamespace

# 健康檢查
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

# 啟動應用程序
ENTRYPOINT ["dotnet", "GameSpace.dll"]