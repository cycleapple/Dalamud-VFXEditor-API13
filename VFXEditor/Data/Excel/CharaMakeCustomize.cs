using Lumina.Excel;
using Lumina.Excel.Sheets;
using VfxEditor.Utils;

namespace VfxEditor.Data.Excel {
    // https://github.com/ktisis-tools/Ktisis/blob/748922b02395ef9a700e3fe446f2fa2d6db0a63f/Ktisis/Data/Excel/CharaMakeCustomize.cs

    [Sheet("CharaMakeCustomize", 3272514419u)]
    public struct CharaMakeCustomize( uint row ) : IExcelRow<CharaMakeCustomize> {
        public ExcelPage ExcelPage { get; private set; }
        public uint RowOffset { get; private set; }
        public readonly uint RowId => row;

        public string Name { get; set; } = "";

        public byte FeatureId { get; set; }
        public uint Icon { get; set; }
        public ushort Data { get; set; }
        public bool IsPurchasable { get; set; }
        public RowRef<Lobby> Hint { get; set; }
        public byte FaceType { get; set; }

        static CharaMakeCustomize IExcelRow<CharaMakeCustomize>.Create( ExcelPage page, uint offset, uint row ) {
            var native = new Lumina.Excel.Sheets.CharaMakeCustomize(page, offset, row);
            return new CharaMakeCustomize( row ) {
                ExcelPage = page,
                RowOffset = offset,
                FeatureId = native.FeatureID,
                Icon = native.Icon,
                Data = native.UnlockLink,
                IsPurchasable = native.IsPurchasable,
                Hint = native.Hint,
                FaceType = native.Unknown0
            };
        }
    }
}
