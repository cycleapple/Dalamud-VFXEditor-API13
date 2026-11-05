using Lumina.Excel;
using System.Collections.Generic;
using System.Linq;
using VfxEditor.Utils;

namespace VfxEditor.Data.Excel {
    [Sheet("HairMakeType", 3374538714u)]
    public struct HairMakeType( uint row ) : IExcelRow<HairMakeType> {
        public ExcelPage ExcelPage { get; private set; }
        public uint RowOffset { get; private set; }
        public readonly uint RowId => row;

        // Properties

        public const uint HairLength = 100;
        public const uint FacepaintLength = 50;

        public uint HairStartIndex { get; set; } // 66
        public uint FacepaintStartIndex { get; set; } // 82

        public List<RowRef<CharaMakeCustomize>> HairStyles { get; set; }
        public List<RowRef<CharaMakeCustomize>> Facepaints { get; set; }

        // Build sheet

        static HairMakeType IExcelRow<HairMakeType>.Create( ExcelPage page, uint offset, uint row ) {
            var native = new Lumina.Excel.Sheets.HairMakeType(page, offset, row);
            var hair = native.CharaMakeStruct[0];
            var face = native.CharaMakeStruct[7];
            foreach (var menu in native.CharaMakeStruct) {
                if (menu.Customize == (uint)global::Dalamud.Game.ClientState.Objects.Enums.CustomizeIndex.HairStyle) hair = menu;
                if (menu.Customize == (uint)global::Dalamud.Game.ClientState.Objects.Enums.CustomizeIndex.Facepaint) face = menu;
            }
            return new HairMakeType(row) {
                ExcelPage = page, RowOffset = offset,
                HairStartIndex = hair.SubMenuParam[0], FacepaintStartIndex = face.SubMenuParam[0],
                HairStyles = hair.SubMenuParam.Where(id => id != 0).Take((int)HairLength).Select(id => new RowRef<CharaMakeCustomize>(page.Module, id, page.Language)).ToList(),
                Facepaints = face.SubMenuParam.Where(id => id != 0).Take((int)FacepaintLength).Select(id => new RowRef<CharaMakeCustomize>(page.Module, id, page.Language)).ToList()
            };
        }

        private static IEnumerable<RowRef<CharaMakeCustomize>> GetRange( ExcelPage page, uint start, uint length ) {
            for( var i = start; i < start + length; i++ )
                yield return new RowRef<CharaMakeCustomize>( page.Module, i, page.Language );
        }
    }
}