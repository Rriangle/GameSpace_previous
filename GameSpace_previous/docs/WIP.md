# WIP 紀錄（嚴格 Re-Audit 回合）

## TODO/佔位掃描（節錄）

- ./README.md:5:### 根目錄 docs/ (稽核與 WIP)
- ./README.md:7:- **WIP.md**: 工作進展追蹤，當前任務狀態
- ./README.md:30:│   ├── docs/               # 稽核與 WIP 文件
- ./README.md:32:│   │   ├── WIP.md
- ./README.md:41:- [工作進展](../GameSpace/docs/WIP.md)
- ./My_Own_Source_Of_Data/old_0905.txt:158:•	骨架屏：內容載入時的佔位動畫
- ./My_Own_Source_Of_Data/CONTRIBUTING_AGENT.txt:24: * UI copy/labels/placeholders
- ./My_Own_Source_Of_Data/CONTRIBUTING_AGENT.txt:39:3) Continuous Run, WIP/Progress & Single-Line Status (put up front)
- ./My_Own_Source_Of_Data/CONTRIBUTING_AGENT.txt:41:Before you touch anything, read docs/WIP_RUN.md (append-only) and docs/PROGRESS.json (per-stage % + overall).
- ./My_Own_Source_Of_Data/CONTRIBUTING_AGENT.txt:47:Delta Plan (C16): Before STOP, append your next mini-plan to docs/WIP_RUN.md → Next (targets, files, tests, risks/assumptions). Do not print the Delta Plan to console.
- ./My_Own_Source_Of_Data/CONTRIBUTING_AGENT.txt:49:> Read and follow CONTRIBUTING_AGENT.txt. Continue from last WIP. Work in small chunks (<=3 files, <=400 LOC). Only stop when token budget is nearly exhausted. On START and on STOP, print exactly one line: 整體進度: <overall>% | 目前階段: <stage>% | 狀態: <short description>.
- ./My_Own_Source_Of_Data/new_0905.txt:338:權限與版主：論壇實施分級管理機制。某些討論版可能設有版主（Moderator），版主是從該遊戲社群中選出的管理者，負責日常內容審核。版主權限包括隱藏違規主題或回覆（將 threads.status 或 thread_posts.status 設為 hidden）、警告或暫時停權違規用戶（通過 User_Rights.User_Status 控制）等。更高層級的超級管理員（後台管理員）則擁有全面權限，可以封禁用戶、刪除內容（設定 status="deleted"）、調整討論版設定（修改 forums.description 等）。對於 threads 和 posts 的隱藏/刪除處理，前端在顯示列表時會自動過濾 status 非正常的內容。例如 threads.status="hidden" 的主題不會出現在一般用戶的列表中；thread_posts.status="deleted" 的回覆在前端可能以「該回覆已被刪除」字樣佔位但不顯示內容。權限檢查在後端執行，未具備版主或管理員權限的用戶試圖調用隱藏/刪除API時將返回403禁止。
- ./My_Own_Source_Of_Data/new_0905.txt:340:文章系統與洞察：除了每款遊戲專屬的討論串，平台還允許發表一般文章，例如官方公告、攻略心得等。這部分內容存放在 posts 資料表。posts 是與論壇平行的發文系統，每篇 post 可不隸屬某特定論壇，但可以關聯一個遊戲以標記主題。欄位包括 post_id（主鍵）、type（文章類型，如 "insight" 表官方洞察文章，"user" 表一般會員文章）、game_id（文章關聯的遊戲，可為 NULL 表示不特定遊戲）、title（文章標題）、tldr（三行摘要，文章的重點提要）、body_md（正文內容 Markdown）、status（狀態: "draft" 草稿、"published" 已發佈、"hidden" 隱藏）、pinned（是否置頂推廣，布林值）、created_by（作者 UserID）、published_at、created_at、updated_at。posts 表上有幾個複合索引，例如 (type, created_at) 用於按照類型過濾最新文章列表、(game_id, created_at) 用於查詢特定遊戲的文章、(status, created_at) 用於後台按狀態篩選文章等。這套文章系統可以用來發佈例如「每周遊戲趨勢報告」（type="insight", game_id=null 表整站分析）、「玩家心得分享」（type="user", game_id=某遊戲）等。用戶可以對文章進行留言評論和按讚收藏，這些互動可重用論壇的回覆與按讚收藏功能（文章評論直接使用 thread_posts 結構來存，以符合現有DB設計，意即文章的留言功能可以將每篇文章視作一個特殊討論串，評論就存入 thread_posts 並以 target_type區分，避免新增新表）。
- ./My_Own_Source_Of_Data/new_0905.txt:352:當玩家想發表新話題時，點擊「發表主題」按鈕進入發帖頁面。在這裡他輸入標題和內容（支持 Markdown 格式編輯），寫好後提交。後端收到請求，會先檢查使用者是否有權在此版發文（檢查 User_Rights.MessagePermission 及是否被該版封禁等）。若通過，則在 threads 表插入新紀錄並返回成功。前端隨即跳轉到新主題頁面，玩家和其他訪問者都能看到他的發文。如果玩家在發帖界面選擇保存草稿而非立即發佈，則 status="draft"，只有他自己可見，在他進入論壇時會顯示「您有未發佈的草稿」提醒，可繼續編輯或發布。
- ./project-docs/AUDIT.md:1:# 嚴格 Re-Audit（No-TODO）
- ./project-docs/AUDIT.md:10:- todo_scan.txt：rg/cloc 未安裝，本輪以 dotnet 輸出與人工稽核為主；已建立空檔以供連結。
- ./project-docs/AUDIT.md:15:## Findings（Zero-Tolerance for TODO/Placeholder）
- ./project-docs/AUDIT.md:38:- 若需持續清空 TODO/佔位，先釐清是否觸及不可變區域，避免違規。
- ./My_Own_Source_Of_Data/index.txt:58:    .grid-tiles{display:grid; gap:12px; grid-template-columns:repeat(6, minmax(0,1fr))}
- ./My_Own_Source_Of_Data/index.txt:59:    @media (max-width:1100px){.grid-tiles{grid-template-columns:repeat(4, minmax(0,1fr))}}
- ./My_Own_Source_Of_Data/index.txt:60:    @media (max-width:740px){.grid-tiles{grid-template-columns:repeat(2, minmax(0,1fr))}}
- ./My_Own_Source_Of_Data/index.txt:87:    .layout{display:grid; grid-template-columns:1fr 360px; gap:16px}
- ./My_Own_Source_Of_Data/index.txt:88:    @media (max-width:980px){.layout{grid-template-columns:1fr}}
- ./My_Own_Source_Of_Data/index.txt:98:    .row{display:grid; grid-template-columns:auto 1fr auto; gap:10px; align-items:center; border:1px solid var(--line); border-radius:14px; padding:10px; background:var(--bg2); transition:transform .16s ease, box-shadow .16s ease}
- ./My_Own_Source_Of_Data/index.txt:120:    .rrow{position:relative; display:grid; grid-template-columns:40px 1fr auto; gap:10px; align-items:center; border:1px solid var(--line); background:var(--bg2); border-radius:12px; padding:10px; overflow:hidden}
- ./My_Own_Source_Of_Data/index.txt:139:    #cats2.grid-tiles{grid-template-columns:repeat(3,minmax(0,1fr))}
- ./My_Own_Source_Of_Data/index.txt:140:    @media (max-width:1100px){#cats2.grid-tiles{grid-template-columns:repeat(2,minmax(0,1fr))}}
- ./My_Own_Source_Of_Data/index.txt:141:    @media (max-width:740px){#cats2.grid-tiles{grid-template-columns:repeat(1,minmax(0,1fr))}}
- ./My_Own_Source_Of_Data/index.txt:171:    .gs-stats{ display:grid; grid-template-columns:1fr; gap:6px; margin-bottom:8px; }
- ./My_Own_Source_Of_Data/index.txt:172:    .gs-stat{ display:grid; grid-template-columns:auto 1fr auto; gap:8px; align-items:center; }
- ./My_Own_Source_Of_Data/index.txt:177:    .gs-actions{ display:grid; grid-template-columns:repeat(5,1fr); gap:6px; margin-bottom:8px; }
- ./My_Own_Source_Of_Data/index.txt:192:        <input id="q2" type="search" placeholder="搜尋文章 / 分區 / 標籤…" aria-label="搜尋"/>
- ./My_Own_Source_Of_Data/index.txt:609:          <label>標題 <input name="title" placeholder="請輸入標題" style="width:100%; padding:10px; border-radius:10px; border:1px solid var(--line); background:var(--surface)"></label>
- ./My_Own_Source_Of_Data/index.txt:611:          <label>標籤（逗號分隔，最多 3 個） <input name="tags" placeholder="#攻略, #情報" style="width:100%; padding:10px; border-radius:10px; border:1px solid var(--line); background:var(--surface)"></label>
- ./project-docs/_archive/20250914-0214/ARCHIVE_INDEX.md:13:2. **WIP.md** - 工作進度記錄
- ./project-docs/_archive/20250914-0214/ARCHIVE_INDEX.md:14:   - 來源: GameSpace/docs/WIP_RUN.md
- ./project-docs/_archive/20250914-0214/AUDIT.md:19:- **TODO/佔位掃描**: 發現 77 個匹配項目，主要為合法的 HTML placeholder 屬性和 CSS grid-template-columns 屬性
- ./project-docs/_archive/20250914-0214/AUDIT.md:24:掃描結果顯示大部分 "placeholder" 匹配為合法的 HTML 表單 placeholder 屬性，這些是正常的 UI 元素，不屬於需要修復的佔位符。
- ./GameSpace/GameSpace.Core/Logging/StructuredLoggingExtensions.cs:30:        public static void LogRetryAttempt(this ILogger logger, int currentAttempt, int maxRetries, double delayMs, string operation, Exception exception)
- ./GameSpace/GameSpace.Core/Logging/StructuredLoggingExtensions.cs:32:            logger.LogWarning("重試執行 {CurrentAttempt}/{MaxRetries}, 延遲 {DelayMs}ms, 操作: {Operation}, 原因: {Reason}",

## 構建與測試摘要
- 見 reports/_latest/dotnet_build.txt
- 見 reports/_latest/dotnet_test.txt

## DB 規格來源
-  為最終權威
\n---\n**2025-09-14T21:27:39Z** — Re-Audit 批次輸出更新
- 產出 reports/_latest：todo_scan/cloc/build/test/db_table_count
- 依不可變規則，Sign-in 相關缺陷登記為 Open（文件化，不改邏輯）
- 2025-09-14T21:37:18Z 新增 docs/ADMIN_TEMPLATE_STYLE.md（SB Admin 使用規範，禁止改 vendor）
- 2025-09-14T21:40:20Z 新增 docs/MODULES.md 與 docs/DATABASE.md（以 database.json 為最終權威）
- 2025-09-14T21:43:29Z 新增 docs/DEPLOYMENT.md、docs/OPERATIONS.md 並更新 README 導航
- 2025-09-14T21:45:40Z 產生 reports/_latest/SCAN_SUMMARY.md 並更新 AUDIT
- 2025-09-14T21:50:25Z 新增 docs/SECURITY.md 與 heartbeat
- 2025-09-14T21:51:57Z 新增 .github/workflows/ci.yml（CI 只讀流程）
- 2025-09-15T03:29:38Z 心跳記錄與檢查點（批次1）
- 2025-09-15T03:29:47Z 產生 BT_SUMMARY.md
- 2025-09-15T03:31:52Z 心跳紀錄（heartbeat-only）」
- 2025-09-15T03:33:27Z 嚴格 TODO 掃描完成（todo_round.txt）
- 2025-09-15T03:33:59Z 嚴格 TODO 掃描完成（todo_grep.txt, grep）
- 2025-09-15T03:34:22Z 產生 BUILD_ERRORS.md
- 2025-09-15T03:34:39Z 建置/測試重試摘要已產出（最多3次）
- 2025-09-15T03:35:35Z Heartbeat 與檢查點（保持連續運行）
- 2025-09-15T03:38:32Z 新增 CONTRIBUTING.md、心跳與檢查點
- 2025-09-15T03:39:21Z 更新 BT_LATEST.md 與 heartbeat
- 2025-09-15T03:40:03Z 更新報告索引 INDEX.md 與 heartbeat
- 2025-09-15T03:40:19Z 產生 STATUS.md 與 heartbeat
- 2025-09-15T03:40:45Z 重建 STATUS.md（遮罩敏感）
- 2025-09-15T03:41:11Z 新增 STYLEGUIDE.md、心跳、檢查點
- 2025-09-15T03:41:37Z 新增 CODING_STANDARDS.md、心跳與檢查點
- 2025-09-15T03:41:59Z 新增 .editorconfig 與 .gitattributes、心跳與檢查點
- 2025-09-15T03:42:27Z 新增 issue templates、心跳與檢查點
- 2025-09-15T03:42:52Z 新增 docs/INDEX.md 與 BT_LATEST.md、heartbeat
- 2025-09-15T03:43:19Z 新增 QA_CHECKLIST、heartbeat 與檢查點
- 2025-09-15T03:43:43Z 新增 .github/SECURITY.md、heartbeat 與檢查點
- 2025-09-15T04:00:21Z 新增 scripts/run_audit.sh（retry）、heartbeat 與檢查點
- 2025-09-15T04:01:07Z 執行 scripts/run_audit.sh 並更新報告
- 2025-09-15T04:02:33Z 更新 README 快速導航（加入索引與規範）、heartbeat 與檢查點
- 2025-09-15T04:03:07Z 新增 CODE_OF_CONDUCT、heartbeat 與檢查點
- 2025-09-15T04:03:39Z 追加 AUDIT 指向最新報告與 heartbeat
- 2025-09-15T04:04:11Z 新增 .gitignore、heartbeat 與檢查點
- 2025-09-15T04:22:49Z 新增 TEST_STRATEGY、heartbeat 與檢查點
