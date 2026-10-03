"""Check the accessor in an archived function export; this is not a full executable scan."""
import json
import re
import sys
from pathlib import Path

source = Path(__file__).resolve().parents[1] / 'VFXEditor/Interop/Constants.cs'
pattern = re.search(r'GetFileManager2Sig = "([^"]+)"', source.read_text(encoding='utf-8')).group(1)
regex = re.compile(b''.join(b'.' if token == '??' else re.escape(bytes.fromhex(token)) for token in pattern.split()), re.S)
for name in sys.argv[1:]:
    data = json.loads(Path(name).read_text(encoding='utf-8'))['functions']
    hits = [(f['address'], match.start()) for f in data.values() for match in regex.finditer(bytes.fromhex(f['bytes']))]
    assert len(hits) == 1 and hits[0][1] == 0, hits
    print(f'{Path(name).name}: one exported accessor entry at {hits[0][0]:#x}; {len(data)} exported functions checked')
