"""Verify the built archive and stage an isolated VFXEditor repository update."""
from pathlib import Path
import hashlib
import json
import shutil
import sys
import zipfile

root = Path(__file__).resolve().parents[1]
repo = Path(sys.argv[1]).resolve()
package = root / 'VFXEditor/bin/Release/VFXEditor/latest.zip'
version = '1.9.2.10'
tag = f'v{version}-api13-zh-tw'
name = f'VFXEditor-{version}.zip'
with zipfile.ZipFile(package) as z:
    assert z.testzip() is None
    names = z.namelist()
    assert len(names) == len(set(names))
    assert all(not n.startswith(('/', '\\')) and '..' not in Path(n).parts for n in names)
    entry = json.loads(z.read('VFXEditor.json').decode('utf-8-sig'))
    assert entry['InternalName'] == 'VFXEditor'
    assert entry['AssemblyVersion'] == version and entry['DalamudApiLevel'] == 13
    for required in ('VFXEditor.dll', 'VFXEditor.deps.json', 'DirectXTexC.dll', 'OtterTex.dll', 'Penumbra.Api.dll', 'Penumbra.String.dll', 'Files/default_vfx.avfx', 'Shaders/Model.fx', 'LICENSE', 'README.zh-TW.md'):
        assert required in names, required
    assert not any(n in names for n in ('Dalamud.dll', 'FFXIVClientStructs.dll', 'Lumina.dll'))
    deps = json.loads(z.read('VFXEditor.deps.json'))
    assert 'Version=v9.0' in deps['runtimeTarget']['name']

entry.update(TestingDalamudApiLevel=13, TestingAssemblyVersion=version, IsHide=False, IsTestingExclusive=False,
    Changelog='修正 GetFileManager2 簽名找不到而無法載入的錯誤：回移上游的函式本體簽名，避開台服不同的呼叫端指令。歷史台服／國際服函式匯出各唯一命中；11 項本地化與 2 組簽名回歸測試、Release 建置及套件檢查通過。請更新後重新啟動遊戲再啟用；尚待回報者遊戲內確認。',
    IconUrl='https://raw.githubusercontent.com/cycleapple/DalamudPlugins-TW-API13-Extended/main/icons/VFXEditor.png')
url = f'https://github.com/cycleapple/DalamudPlugins-TW-API13-Extended/releases/download/plugins-api13/{name}'
entry.update(DownloadLinkInstall=url, DownloadLinkUpdate=url, DownloadLinkTesting=url)
manifest = repo / 'repo.json'
original_text = manifest.read_text(encoding='utf-8-sig')
original = json.loads(original_text)
assert len([x for x in original if x['InternalName'] == 'VFXEditor']) == 1
# Locate the existing JSON object, preserving all bytes outside this one entry.
decoder = json.JSONDecoder()
position = original_text.index('[') + 1
while True:
    while original_text[position] in ' \t\r\n,': position += 1
    item, end = decoder.raw_decode(original_text, position)
    if item['InternalName'] == 'VFXEditor':
        assert tuple(map(int, item['AssemblyVersion'].split('.'))) < tuple(map(int, version.split('.')))
        entry_text = json.dumps(entry, ensure_ascii=False, indent=2).replace('\n', '\n  ')
        new_text = original_text[:position] + entry_text + original_text[end:]
        break
    position = end
assert [x for x in json.loads(new_text) if x['InternalName'] != 'VFXEditor'] == [x for x in original if x['InternalName'] != 'VFXEditor']
manifest.write_text(new_text, encoding='utf-8')
shutil.copy2(root / 'assets/icon.png', repo / 'icons/VFXEditor.png')
for filename, content in {
    'README.md': '\n## VFXEditor 1.9.2.10 載入修正\n\n修正 GetFileManager2 簽名找不到導致無法載入的問題。回移上游函式本體簽名，保留 API 13 與繁中介面。已通過建置、11 項本地化及 2 組簽名測試；更新後請重新啟動遊戲再啟用。尚待回報者遊戲內確認。\n',
    'SOURCE_AUDIT.md': '\n## VFXEditor 1.9.2.10\n\n回移上游 `fbbfedb4` 的 GetFileManager2 函式本體簽名。2025-12-02 歷史函式匯出：台服 98,845 個函式中唯一入口 0x140455BA0；國際服 97,901 個函式中唯一入口 0x1401E6B00。保留 API 13／.NET 9，新增兩組入口簽名回歸測試。未取得回報者目前執行檔，以上並非完整客戶端掃描或遊戲內實測。\n',
    'LOCALIZATION_AUDIT.md': '\n## VFXEditor 1.9.2.10\n\n651 個 zh-TW 詞條；主要選單、設定、檔案選擇、匯入／匯出與常见控制項已接入顯示層。遊戲路徑、工作區鍵與檔案格式不翻譯，部分動態提示及進階欄位仍為英文。11 項本地化測試通過；DLL 已確認參照 API 13 與 .NET 9。尚未驗證遊戲內字型、載入與資源替換功能。\n',
}.items():
    f = repo / filename
    with f.open('a', encoding='utf-8') as out: out.write(content.replace('常见', '常見'))
fork_entry = dict(entry)
fork_url = f'https://github.com/cycleapple/Dalamud-VFXEditor-API13/releases/download/{tag}/{name}'
fork_entry.update(DownloadLinkInstall=fork_url, DownloadLinkUpdate=fork_url, DownloadLinkTesting=fork_url,
    IconUrl='https://raw.githubusercontent.com/cycleapple/Dalamud-VFXEditor-API13/api13-zh-tw/assets/icon.png')
(root / 'repo.json').write_text(json.dumps([fork_entry], ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
stage = root / 'artifacts'
stage.mkdir(exist_ok=True)
shutil.copy2(package, stage / name)
digest = hashlib.sha256(package.read_bytes()).hexdigest()
(stage / 'SHA256SUMS.txt').write_text(f'{digest}  {name}\n', encoding='utf-8')
(stage / 'release-notes.txt').write_text(f'VFXEditor {version} API 13／繁體中文版本。\n\n{entry["Changelog"]}\n\n安裝後使用 /vfxedit 開啟。沿用 VFXEditor InternalName，請勿同時啟用上游版本。\n\nSHA256：{digest}\n', encoding='utf-8')
print(f'Validated {len(names)} packaged files; updated only VFXEditor among {len(original)} entries. SHA256 {digest}')
