這個 repo 要實作一個 .NET 8 的 QVR Pro API Client Library，封裝 QVR Pro Server / Elite 的 REST API 與 CGI（例如 qvr_pro_api_1.3.1.yaml 與 /qvrpro/apis/qplay.cgi）。

## 建議的資料夾結構
- src/QvrProClient
  - Api：集中管理 endpoint path 與 API 常數。
  - DependencyInjection：DI/HttpClientFactory 註冊入口。
  - Exceptions：將 QVR Pro 錯誤碼/HTTP 狀態轉換成自訂 Exception。
  - Http：HttpClient 包裝、回應解析與錯誤處理。
  - Models：Swagger/yaml 對應的 DTO、Qplay session、錄影資訊等。
  - Options：QVR Pro 連線與預設登入設定。
  - Services：IQvrProClient 的預設實作與未來子服務（例如 Playback/Export）。

## 主要 Public API（初稿）
- `IQvrProClient`
  - `Task LoginAsync(string username, string password, CancellationToken ct = default)`
  - `Task LogoutAsync(CancellationToken ct = default)`
  - `Task<IReadOnlyList<QvrCameraInfo>> GetCamerasAsync(CancellationToken ct = default)`
  - `Task<QvrRecordingInfo?> GetRecordingInfoAsync(string cameraId, DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default)`
  - `Task<QplaySession> OpenPlaybackAsync(string cameraId, DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default)`
  - `Task ExportRecordingAsync(string cameraId, DateTimeOffset from, DateTimeOffset to, CancellationToken ct = default)`
- 後續可分拆子介面：IPlaybackClient（open/play/close/getstreaminfo）、IExportClient（建立/輪詢匯出任務）、IRecordingClient（查詢錄影範圍）。

## Options/設定類別
- `QvrProOptions`
  - `Host`, `Port`, `UseHttps`, `IgnoreCertificateErrors`
  - `DefaultUsername`, `DefaultPassword`
  - `Timeout`
  - `BuildBaseUri()`：根據 host/port/https 建立 base 地址。

## 自動/半自動產生 Models 與 Requests
1. 使用 qvr_pro_api_x.y.z.yaml：
   - 可使用 [NSwag](https://github.com/RicoSuter/NSwag) CLI 或 [Kiota](https://github.com/microsoft/kiota) 從 OpenAPI YAML 產生 C# DTO 與 Client stub。
   - 範例（有網路環境時）：`nswag openapi2csclient /input:qvr_pro_api_1.3.1.yaml /classname:QvrProGeneratedClient /namespace:QvrProClient.Generated /output:src/QvrProClient/Generated/QvrProGeneratedClient.cs`
   - 產生後可放在 `src/QvrProClient/Generated`，並在 Services 中包裝成高階 API。
2. Qplay CGI 沒有 swagger，可以手寫 request/response model，或用 partial class 與 request builder 封裝。
3. 產生出的 models 可再透過 partial class、擴充方法轉換成高階 domain model（例如 QvrRecordingInfo）。

## 目前新增的初始檔案
- Solution: `QvrProClient.sln`
- Project: `src/QvrProClient/QvrProClient.csproj`
- Public API：`src/QvrProClient/IQvrProClient.cs`
- Options：`src/QvrProClient/Options/QvrProOptions.cs`
- Http 包裝：`src/QvrProClient/Http/QvrProHttpClient.cs`
- Exception：`src/QvrProClient/Exceptions/QvrProException.cs`
- Models (基本 DTO)：`src/QvrProClient/Models/*.cs`
- 預設實作 stub：`src/QvrProClient/Services/QvrProClient.cs`
- DI 註冊：`src/QvrProClient/DependencyInjection/ServiceCollectionExtensions.cs`
- API 常數：`src/QvrProClient/Api/QvrProRoutes.cs`

## 下一步建議
1. 依 swagger yaml 產生 DTO 與 request/response 類別，移到 `Generated/`。
2. 依 QVR Pro 認證流程，於 `QvrProClient` 實作 Session cookie/token 管理並與 HttpClientHandler 整合。
3. 補齊 Playback/Export 的細部 API 呼叫與資料型別，並撰寫整合測試（可對接模擬伺服器）。
