using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using System.Numerics;

namespace VfxEditor.Formats.MtrlFormat.Stm {
    public class StmDyeData {
        public Vector3 Diffuse = new();
        public Vector3 Specular = new();
        public Vector3 Emissive = new();
        public float Gloss = 0;
        public float Power = 0;

        public void Draw() {
            ImGui.ColorEdit3( VfxEditor.Localization.ZhTw.Label( "##Diffuse" ), ref Diffuse, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRgb | ImGuiColorEditFlags.InputRgb | ImGuiColorEditFlags.NoTooltip );

            ImGui.SameLine();
            ImGui.ColorEdit3( VfxEditor.Localization.ZhTw.Label( "##Specular" ), ref Specular, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRgb | ImGuiColorEditFlags.InputRgb | ImGuiColorEditFlags.NoTooltip );

            ImGui.SameLine();
            ImGui.ColorEdit3( VfxEditor.Localization.ZhTw.Label( "##Emissive" ), ref Emissive, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.DisplayRgb | ImGuiColorEditFlags.InputRgb | ImGuiColorEditFlags.NoTooltip );

            using var disabled = ImRaii.Disabled();
            ImGui.SameLine();
            ImGui.SetNextItemWidth( 75 );
            ImGui.InputFloat( VfxEditor.Localization.ZhTw.Label( "##Gloss" ), ref Gloss, 0, 0, $"Gloss: %.1f", ImGuiInputTextFlags.ReadOnly );
            ImGui.SameLine();
            ImGui.SetNextItemWidth( 75 );
            ImGui.InputFloat( VfxEditor.Localization.ZhTw.Label( "##Power" ), ref Gloss, 0, 0, $"Power: %.1f", ImGuiInputTextFlags.ReadOnly );
        }
    }
}
