using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace VfxEditor.Localization {
    /// <summary>Display-only translations. Original strings remain the ImGui IDs and serialized values.</summary>
    public static class ZhTw {
        private static readonly Dictionary<string, string> Strings = Load();

        private static Dictionary<string, string> Load() {
            using var stream = typeof(ZhTw).Assembly.GetManifestResourceStream("VfxEditor.Localization.zh-TW.json")
                ?? throw new InvalidDataException("Missing embedded zh-TW translations");
            return JsonSerializer.Deserialize<Dictionary<string, string>>(stream)
                ?? throw new InvalidDataException("Invalid zh-TW translations");
        }

        public static string Text(string value) {
            if (string.IsNullOrEmpty(value)) return value;
            return Strings.TryGetValue(value, out var translated) ? translated : value;
        }

        public static string Label(string value) {
            if (string.IsNullOrEmpty(value) || value.StartsWith("##", StringComparison.Ordinal)) return value;
            var separator = value.IndexOf("##", StringComparison.Ordinal);
            var visible = separator < 0 ? value : value[..separator];
            var translated = Text(visible);
            if (translated == visible) return value;
            // ### resets ImGui's hash. Preserve an existing ### identity, or the complete original label.
            var stableId = value.IndexOf("###", StringComparison.Ordinal);
            return translated + "###" + (stableId < 0 ? value : value[(stableId + 3)..]);
        }
    }
}
