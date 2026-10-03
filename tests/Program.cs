using VfxEditor.Localization;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

var cases = new (string actual, string expected)[] {
    (ZhTw.Text("Save"), "儲存"),
    (ZhTw.Text("vfx/common/eff/example.avfx"), "vfx/common/eff/example.avfx"),
    (ZhTw.Text("A user-created file"), "A user-created file"),
    (ZhTw.Label("Save"), "儲存###Save"),
    (ZhTw.Label("Save##document1"), "儲存###Save##document1"),
    (ZhTw.Label("Save###document1"), "儲存###document1"),
    (ZhTw.Label("##Hidden"), "##Hidden"),
    (ZhTw.Label(""), ""),
    (ZhTw.Label("User Label###id"), "User Label###id"),
    (ZhTw.Label(ZhTw.Label("Save")), ZhTw.Label("Save")),
};
foreach (var (actual, expected) in cases)
    if (actual != expected) throw new Exception($"Expected '{expected}', got '{actual}'");
if (ZhTw.Label("Add##a") == ZhTw.Label("Add##b"))
    throw new Exception("Distinct control IDs collapsed");
Console.WriteLine($"Passed {cases.Length + 1} localization checks (resource, fallback, hidden IDs, suffixes, idempotence).");

// Archived December 2025 disassembly prefixes. The accessor starts at byte zero;
// no CALL instruction should be resolved when ScanText returns this signature.
var accessorPrefixes = new[] {
    "4883ec28488b059db34b024885c00f850901000048895c24304533c033d248897c2420b9b8000000e8133ec3ff33db488bf84885c00f8490000000488d050edbbd0148897424384889078d73078bc387470848893d47b34b024883c7480f1f004533c033d2b900400400e8e13dc3ff4885c0741bb900220000488d50180f1f00",
    "4883ec28488b05ad7654024885c00f850901000048895c24304533c033d248897c2420b9b8000000e843a3e7ff33db488bf84885c00f8490000000488d052ebde00148897424384889078d73078bc387470848893d577654024883c7480f1f004533c033d2b900400400e811a3e7ff4885c0741bb900220000488d50180f1f00",
};
var pattern = VfxEditor.Interop.Constants.GetFileManager2Sig.Split(' ');
foreach (var prefix in accessorPrefixes) {
    var bytes = Convert.FromHexString(prefix);
    for (var i = 0; i < pattern.Length; ++i)
        if (pattern[i] != "??" && Convert.ToByte(pattern[i], 16) != bytes[i])
            throw new Exception($"GetFileManager2 no longer matches accessor entry at byte {i}");
}
if (pattern[0] == "E8") throw new Exception("GetFileManager2 must match the accessor body");
Console.WriteLine("Passed 2 GetFileManager2 regression fixtures (TC and Global accessor entry points).");

if (args.Length > 0) {
    using var file = File.OpenRead(args[0]);
    using var pe = new PEReader(file);
    var metadata = pe.GetMetadataReader();
    var assembly = metadata.GetAssemblyDefinition();
    if (assembly.Version != new Version(1, 9, 2, 10)) throw new Exception("Wrong plugin assembly version");
    var refs = metadata.AssemblyReferences.Select(h => metadata.GetAssemblyReference(h)).ToArray();
    var dalamud = refs.Single(r => metadata.GetString(r.Name) == "Dalamud");
    if (dalamud.Version.Major != 13) throw new Exception("Wrong Dalamud reference");
    var runtime = refs.Single(r => metadata.GetString(r.Name) == "System.Runtime");
    if (runtime.Version.Major != 9) throw new Exception("Wrong .NET reference");
    if (!metadata.ManifestResources.Any(h => metadata.GetString(metadata.GetManifestResource(h).Name) == "VfxEditor.Localization.zh-TW.json"))
        throw new Exception("Missing translations in actual plugin");
    Console.WriteLine($"Verified actual assembly: {assembly.Version}, Dalamud {dalamud.Version}, .NET {runtime.Version}, embedded zh-TW.");
}
