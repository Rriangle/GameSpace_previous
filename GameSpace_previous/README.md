# GameSpace 專案

## 文件入口規則

### 根目錄 docs/ (稽核與 WIP)
- **AUDIT.md**: 專案審計報告，記錄所有問題和修復狀態
- **WIP.md**: 工作進展追蹤，當前任務狀態
- **_archive/**: 歷史歸檔，按時間戳組織

### GameSpace_previous/docs/ (架構/部署/測試等技術文)
- **SYSTEM_ARCHITECTURE.md**: 系統架構設計
- **DEPLOYMENT_CHECKLIST.md**: 部署檢查清單
- **TESTING_POLICY.md**: 測試政策與規範
- **PERFORMANCE_BUDGET.md**: 效能預算與優化指南
- **OPERATIONS.md**: 營運相關文件

### GameSpace.Api / GameSpace.* 專案內 docs/ (技術/模組說明)
- **API 文檔**: 各專案內的技術規格與模組說明
- **交叉連結**: 與根目錄和專案文檔互相引用

## 專案結構

```
GameSpace_previous/
├── docs/                    # 技術規格與架構文件
│   ├── SYSTEM_ARCHITECTURE.md
│   ├── DEPLOYMENT_CHECKLIST.md
│   └── ...
├── GameSpace/              # 主要專案代碼
│   ├── docs/               # 稽核與 WIP 文件
│   │   ├── AUDIT.md
│   │   ├── WIP.md
│   │   └── _archive/
│   └── ...
└── README.md               # 本文件
```

## 快速導航

- [審計報告](../GameSpace/docs/AUDIT.md)
- [工作進展](../GameSpace/docs/WIP.md)
- [系統架構](docs/SYSTEM_ARCHITECTURE.md)
- [部署指南](docs/DEPLOYMENT_CHECKLIST.md)
- [測試政策](docs/TESTING_POLICY.md)

## 開發規範

- 所有文件使用繁體中文
- 遵循 Git 提交規範
- 微批次開發：每次 ≤ 3 檔或 ≤ 400 行
- 每批次需通過 `dotnet build` 驗證