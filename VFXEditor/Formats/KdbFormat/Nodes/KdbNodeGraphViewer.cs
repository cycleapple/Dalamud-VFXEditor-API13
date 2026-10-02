using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using VfxEditor.Formats.KdbFormat.Nodes.Types.Effector;
using VfxEditor.Formats.KdbFormat.Nodes.Types.Source;
using VfxEditor.Formats.KdbFormat.Nodes.Types.Target;
using VfxEditor.Formats.KdbFormat.Nodes.Types.TargetConstraint;
using VfxEditor.Ui.NodeGraphViewer;

namespace VfxEditor.Formats.KdbFormat.Nodes {
    public class KdbNodeGraphViewer : NodeGraphViewer<KdbNode, KdbSlot> {
        public KdbNodeGraphViewer() { }

        protected override void DrawUtilsBar() {
            using( var font = ImRaii.PushFont( UiBuilder.IconFont ) ) {
                if( ImGui.Button( FontAwesomeIcon.Plus.ToIconString() ) ) {
                    ImGui.OpenPopup( "NewPopup" );
                }
            }

            using( var popup = ImRaii.Popup( "NewPopup" ) ) {
                if( popup ) {
                    if( ImGui.Selectable( VfxEditor.Localization.ZhTw.Label( "EffectorExpr" ) ) ) AddToCanvas( new KdbNodeEffectorExpr(), true );
                    if( ImGui.Selectable( VfxEditor.Localization.ZhTw.Label( "EffectorEZParamLink" ) ) ) AddToCanvas( new KdbNodeEffectorEZParamLink(), true );
                    if( ImGui.Selectable( VfxEditor.Localization.ZhTw.Label( "EffectorEZParamLinkLinear" ) ) ) AddToCanvas( new KdbNodeEffectorEZParamLinkLinear(), true );
                    if( ImGui.Selectable( VfxEditor.Localization.ZhTw.Label( "SourceRotate" ) ) ) AddToCanvas( new KdbNodeSourceRotate(), true );
                    if( ImGui.Selectable( VfxEditor.Localization.ZhTw.Label( "SourceTranslate" ) ) ) AddToCanvas( new KdbNodeSourceTranslate(), true );
                    if( ImGui.Selectable( VfxEditor.Localization.ZhTw.Label( "TargetBendSTRoll" ) ) ) AddToCanvas( new KdbNodeTargetBendSTRoll(), true );
                    if( ImGui.Selectable( VfxEditor.Localization.ZhTw.Label( "TargetBendRoll" ) ) ) AddToCanvas( new KdbNodeTargetBendRoll(), true );
                    if( ImGui.Selectable( VfxEditor.Localization.ZhTw.Label( "TargetTranslate" ) ) ) AddToCanvas( new KdbNodeTargetTranslate(), true );
                    if( ImGui.Selectable( VfxEditor.Localization.ZhTw.Label( "TargetScale" ) ) ) AddToCanvas( new KdbNodeTargetScale(), true );
                    if( ImGui.Selectable( VfxEditor.Localization.ZhTw.Label( "TargetRotate" ) ) ) AddToCanvas( new KdbNodeTargetRotate(), true );
                    if( ImGui.Selectable( VfxEditor.Localization.ZhTw.Label( "TargetPosConstraint" ) ) ) AddToCanvas( new KdbNodeTargetPosConstraint(), true );
                    if( ImGui.Selectable( VfxEditor.Localization.ZhTw.Label( "TargetOrientationConstraint" ) ) ) AddToCanvas( new KdbNodeTargetOrientationConstraint(), true );
                }
            }

            ImGui.SameLine();
            // ===================
            base.DrawUtilsBar();
        }
    }
}
