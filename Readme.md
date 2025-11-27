這個 repo 要實作一個 .NET 8 的 QVR Pro API Client Library
封裝 QVR Pro Server / Elite 的 REST API 與 CGI（例如 qvr_pro_api_1.3.1.yaml 與 /qvrpro/apis/qplay.cgi）。
目標是提供一個 IQvrProClient 介面，可以：

Login / Logout（維護 Session）https://github.com/FrankLuPcEng/QvrClient/tree/main

查詢攝影機列表與錄影資訊

控制即時與回放串流（包含 qplay open/play/close 流程）

建立與監控錄影匯出任務
使用 HttpClient + JSON，支援 DI 與可測試性。
