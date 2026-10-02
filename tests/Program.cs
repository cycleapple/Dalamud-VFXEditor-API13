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

if (args.Length > 0) {
    using var file = File.OpenRead(args[0]);
    using var pe = new PEReader(file);
    var metadata = pe.GetMetadataReader();
    var assembly = metadata.GetAssemblyDefinition();
    if (assembly.Version != new Version(1, 9, 2, 9)) throw new Exception("Wrong plugin assembly version");
    var refs = metadata.AssemblyReferences.Select(h => metadata.GetAssemblyReference(h)).ToArray();
    var dalamud = refs.Single(r => metadata.GetString(r.Name) == "Dalamud");
    if (dalamud.Version.Major != 13) throw new Exception("Wrong Dalamud reference");
    var runtime = refs.Single(r => metadata.GetString(r.Name) == "System.Runtime");
    if (runtime.Version.Major != 9) throw new Exception("Wrong .NET reference");
    if (!metadata.ManifestResources.Any(h => metadata.GetString(metadata.GetManifestResource(h).Name) == "VfxEditor.Localization.zh-TW.json"))
        throw new Exception("Missing translations in actual plugin");
    Console.WriteLine($"Verified actual assembly: {assembly.Version}, Dalamud {dalamud.Version}, .NET {runtime.Version}, embedded zh-TW.");
}
