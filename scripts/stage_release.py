"""Verify the built archive and stage an additive extended-repository update."""
from pathlib import Path
import hashlib
import json
import shutil
import sys
import zipfile

root = Path(__file__).resolve().parents[1]
repo = Path(sys.argv[1]).resolve()
package = root / 'VFXEditor/bin/Release/VFXEditor/latest.zip'
version = '1.9.2.9'
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
    Changelog='API 13／.NET 9 繁體中文版本。以最後一個上游 API 13 主線節點為基底；651 個翻譯詞條，主要選單、設定及編輯操作繁中化，部分進階欄位保留英文。Release 建置、11 項本地化測試、DLL 參照及套件檢查通過；尚未完成繁中客戶端遊戲內實測。',
    IconUrl='https://raw.githubusercontent.com/cycleapple/DalamudPlugins-TW-API13-Extended/main/icons/VFXEditor.png')
url = f'https://github.com/cycleapple/DalamudPlugins-TW-API13-Extended/releases/download/plugins-api13/{name}'
entry.update(DownloadLinkInstall=url, DownloadLinkUpdate=url, DownloadLinkTesting=url)
manifest = repo / 'repo.json'
original_text = manifest.read_text(encoding='utf-8-sig')
original = json.loads(original_text)
assert not any(x['InternalName'] == 'VFXEditor' for x in original), 'Entry already exists'
closing = original_text.rfind(']')
new_text = original_text[:closing].rstrip() + ',\n' + '\n'.join('  ' + line for line in json.dumps(entry, ensure_ascii=False, indent=2).splitlines()) + '\n]\n'
assert json.loads(new_text)[:-1] == original
manifest.write_text(new_text, encoding='utf-8')
shutil.copy2(root / 'assets/icon.png', repo / 'icons/VFXEditor.png')
for filename, content in {
    'README.md': '\n## VFXEditor API 13 繁中版\n\n新增 VFXEditor 1.9.2.9，651 個繁中詞條，主要介面繁中化。以最後一版上游 API 13 主線原始碼為基底，部分進階欄位保留英文。已完成 Release 建置、本地化及套件檢查；尚未完成遊戲內實測。指令：`/vfxedit`。\n',
    'SOURCE_AUDIT.md': '\n## VFXEditor 1.9.2.9\n\n來源：https://github.com/cycleapple/Dalamud-VFXEditor-API13/tree/api13-zh-tw 。上游基底：0ceal0t/Dalamud-VFXEditor `c320f08c`（1.9.2.8，API 14 更新之前）。以 Dalamud 13.0.0.6／.NET 9 建置；遊戲掛鉤及 Penumbra IPC 沿用上游 API 13，尚未遊戲內驗證。\n',
    'LOCALIZATION_AUDIT.md': '\n## VFXEditor 1.9.2.9\n\n651 個 zh-TW 詞條；主要選單、設定、檔案選擇、匯入／匯出與常见控制項已接入顯示層。遊戲路徑、工作區鍵與檔案格式不翻譯，部分動態提示及進階欄位仍為英文。11 項本地化測試通過；DLL 已確認參照 API 13 與 .NET 9。尚未驗證遊戲內字型、載入與資源替換功能。\n',
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
print(f'Validated {len(names)} packaged files; {len(original)} existing repository entries unchanged. SHA256 {digest}')
