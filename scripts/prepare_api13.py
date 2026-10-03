from pathlib import Path
import json
import re

root = Path(__file__).resolve().parents[1]
p = root / 'VFXEditor'
translations = dict(line.split('\t', 1) for line in (root / 'scripts/zh-TW.tsv').read_text(encoding='utf-8').splitlines() if line)
for kind in ('File', 'Atch', 'Eid', 'Shcd', 'Pbd', 'Uld', 'Sklb', 'Skp', 'Shpk', 'Kdb', 'Pap', 'Scd', 'Tmb', 'Mtrl', 'Phyb', 'Mdl', 'Texture'):
    name = '檔案' if kind == 'File' else kind
    translations[f'{kind} Select [LOADED]'] = f'選擇 {name}［載入來源］'
    translations[f'{kind} Select [REPLACED]'] = f'選擇 {name}［取代目標］'
(p / 'Localization/zh-TW.json').write_text(json.dumps(translations, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
project = p / 'VFXEditor.csproj'
s = project.read_text(encoding='utf-8-sig').replace('1.9.2.8', '1.9.2.10')
if 'LogicalName="VfxEditor.Localization.zh-TW.json"' not in s:
    s = s.replace('</Project>', '    <ItemGroup>\n        <EmbeddedResource Include="Localization/zh-TW.json" LogicalName="VfxEditor.Localization.zh-TW.json" />\n    </ItemGroup>\n</Project>')
project.write_text(s, encoding='utf-8')
manifest = p / 'VFXEditor.json'
data = json.loads(manifest.read_text(encoding='utf-8-sig'))
data.update(Name='VFXEditor（繁體中文）', AssemblyVersion='1.9.2.10', DalamudApiLevel=13,
    Author='ocealot / cycleapple（API 13 繁中版）',
    Punchline='特效、動畫與音效編輯器（API 13／繁體中文）',
    Description='編輯及預覽 FFXIV 特效、動畫、音效、貼圖與模型。以最後一版上游 API 13 原始碼為基底，主要操作介面為繁體中文，部分進階格式欄位保留英文。圖示：PAPACHIN。尚未完成繁中客戶端遊戲內實測。',
    RepoUrl='https://github.com/cycleapple/Dalamud-VFXEditor-API13', AcceptsFeedback=False)
manifest.write_text(json.dumps(data, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
for relative, before, after in [
    ('Ui/DalamudWindow.cs', 'base( name,', 'base( VfxEditor.Localization.ZhTw.Label( name ),'),
    ('Ui/Components/Modal.cs', 'Title = title;', 'Title = VfxEditor.Localization.ZhTw.Label( title );'),
    ('Plugin.cs', 'HelpMessage = "toggle ui"', 'HelpMessage = "開啟或關閉特效編輯器"'),
    ('Plugin.Ui.cs', 'https://github.com/0ceal0t/Dalamud-VFXEditor/issues', 'https://github.com/cycleapple/Dalamud-VFXEditor-API13/issues'),
]:
    f = p / relative
    f.write_text(f.read_text(encoding='utf-8-sig').replace(before, after), encoding='utf-8')
missing = set()
for f in p.rglob('*.cs'):
    if 'obj' in f.parts or 'bin' in f.parts: continue
    for key in re.findall(r'ZhTw\.(?:Label|Text)\( "([^"\\]*)" \)', f.read_text(encoding='utf-8-sig')):
        key = key.split('##')[0]
        if key and key not in translations: missing.add(key)
(root / 'scripts/untranslated-ui.txt').write_text('\n'.join(sorted(missing)), encoding='utf-8')
print(f'{len(translations)} translations, {len(missing)} remaining literal labels')
