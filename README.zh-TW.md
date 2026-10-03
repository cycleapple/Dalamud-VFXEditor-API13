# VFXEditor：API 13 繁體中文版

以 ocealot 的 VFXEditor API 13 最後一個主線節點 `c320f08c` 為基底（上游版本 1.9.2.8，7.4／API 14 更新之前），此分支版本為 **1.9.2.10**，目標為 **Dalamud API 13／.NET 9**。

此版本不是上游 API 15 最新版的完整功能回移。保留上游 MIT 授權及原作者資訊。

## 安裝

在 Dalamud 的自訂插件庫加入：

```text
https://raw.githubusercontent.com/cycleapple/DalamudPlugins-TW-API13-Extended/main/repo.json
```

安裝「VFXEditor（繁體中文）」，使用 `/vfxedit` 開啟。InternalName 沿用 `VFXEditor`；請勿同時啟用其他版本的 VFXEditor。

## 繁中範圍

主要選單、設定、檔案選擇、匯入／匯出、常見編輯控制項與提示採用繁體中文。翻譯只在顯示時套用，保留工作區欄位、檔案路徑、格式代碼及原始資料。未翻譯的進階欄位、部分動態提示與專有名稱會回退英文；不宣稱所有格式欄位已完成翻譯。

`VFXEditor/Localization/zh-TW.json` 是嵌入式詞典；`scripts/zh-TW.tsv` 為維護來源。文字輸出與 ImGui 控制項分別處理，已帶有 `###` 的識別碼保持一致，隱藏標籤不翻譯。

## 建置與驗證

```powershell
git submodule update --init --recursive
./scripts/Build-Api13.ps1 -DalamudLibPath 'C:/path/to/api13/dalamud'
```

需 .NET 9 SDK 和真正的 Dalamud API 13 SDK；腳本會拒絕其他主要版本。產物位於 `VFXEditor/bin/Release/VFXEditor/latest.zip`。CI 只執行不依賴遊戲的本地化檢查，不會自動抓取目前的 API 15 SDK。

已以 Dalamud 13.0.0.6 建置；保留上游的 nullable 警告。測試涵蓋詞典載入、未知文字回退、隱藏 ID、`##`／`###` 識別碼與重複套用。

**尚未完成繁中客戶端遊戲內實測。** GetFileManager2 簽名已回移上游修正，其餘掛鉤、遊戲結構及 Penumbra IPC 沿用上游 API 13 基底；編譯成功不代表每項資源替換、生成特效、動畫／音效預覽均已驗證。需實測插件載入、`/vfxedit`、設定、載入 AVFX、預覽及匯出／重新開啟工作區。

圖示：PAPACHIN。原始專案與功能指南見 [上游 VFXEditor](https://github.com/0ceal0t/Dalamud-VFXEditor)。

## 1.9.2.10 載入修正

修正 LOG 中 `GetFileManager2Sig` 找不到、導致 ResourceLoader 建構失敗的問題。回移上游 `fbbfedb4` 的函式本體簽名，避免依賴台服已變更的呼叫端指令。以 2025-12-02 保存的台服／國際服函式匯出資料確認各有一個入口吻合，並加入兩組回歸測試。這是歷史反組譯資料的驗證，非使用者目前客戶端的完整執行檔掃描或遊戲內實測。

遇過載入失敗者請更新至 1.9.2.10，重新啟動遊戲後再啟用。
