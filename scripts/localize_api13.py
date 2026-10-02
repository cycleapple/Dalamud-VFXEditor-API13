"""Apply display-only localization at the ImGui boundary. Never translate data keys."""
from pathlib import Path
import re

root = Path(__file__).resolve().parents[1] / 'VFXEditor'
labels = 'BeginMenu MenuItem Button SmallButton Checkbox RadioButton InputText InputTextMultiline InputInt InputInt2 InputFloat InputFloat2 InputFloat3 InputFloat4 DragFloat DragFloat2 DragFloat3 DragFloat4 DragInt SliderFloat SliderInt ColorEdit3 ColorEdit4 ColorPicker4 BeginTabItem CollapsingHeader BeginCombo Selectable TreeNode TreeNodeEx TableSetupColumn'.split()
texts = 'Text TextWrapped TextDisabled TextUnformatted SetTooltip BulletText SeparatorText CalcTextSize'.split()
# Only literals and simple label identifiers are rewritten; arbitrary expressions remain intact.
arg = r'(?P<arg>"(?:[^"\\]|\\.)*"|(?:Name|name|Label|label|Title|title|text|Text|Header|header))(?=\s*[,\)])'
patterns = [(re.compile(r'(ImGui\.(?:' + '|'.join(labels) + r')\(\s*)' + arg), 'Label'),
            (re.compile(r'(ImGui\.(?:' + '|'.join(texts) + r')\(\s*)' + arg), 'Text'),
            (re.compile(r'(ImRaii\.(?:TabItem|Combo|TreeNode)\(\s*)' + arg), 'Label'),
            (re.compile(r'(ImGui\.InputTextWithHint\(\s*"(?:[^"\\]|\\.)*"\s*,\s*)' + arg), 'Text')]
count = 0
for path in root.rglob('*.cs'):
    if any(p in path.parts for p in ('obj', 'bin', 'Localization')):
        continue
    original = path.read_text(encoding='utf-8-sig')
    updated = original
    for pattern, method in patterns:
        updated, n = pattern.subn(lambda m: m[1] + 'VfxEditor.Localization.ZhTw.' + method + '( ' + m['arg'] + ' )', updated)
        count += n
    if updated != original:
        path.write_text(updated, encoding='utf-8')
print(f'Localized {count} display call sites')
