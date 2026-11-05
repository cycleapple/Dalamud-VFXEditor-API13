using Dalamud.Game.ClientState.Objects.Enums;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using VfxEditor.Utils;

namespace VfxEditor.Data.Excel {
    // https://github.com/ktisis-tools/Ktisis/blob/748922b02395ef9a700e3fe446f2fa2d6db0a63f/Ktisis/Data/Excel/CharaMakeType.cs

    public enum MenuType : byte {
        List = 0,
        Select = 1,
        Color = 2,
        Unknown1 = 3,
        SelectMulti = 4,
        Slider = 5
    }

    public struct Menu {
        public string Name;
        public byte Default;
        public MenuType Type;
        public byte Count;
        // LookAt
        // SubMenuMask
        public CustomizeIndex Index;
        public uint[] Params;
        public byte[] Graphics;

        public RowRef<CharaMakeCustomize>[] Features;

        public readonly bool HasIcon => Type == MenuType.Select;
        public readonly bool IsFeature => HasIcon && Graphics[0] == 0;
    }

    [Sheet( "CharaMakeType", 0x50CDBEEF )]
    public struct CharaMakeType( uint row ) : IExcelRow<CharaMakeType> {
        public ExcelPage ExcelPage { get; private set; }
        public uint RowOffset { get; private set; }
        // Consts

        public const int MenuCt = 28;
        public const int VoiceCt = 12;
        public const int GraphicCt = 10;
        public const int FacialFeaturesCt = 7 * 8;

        // Properties

        public readonly uint RowId => row;

        public RowRef<Race> Race { get; set; }
        public RowRef<Tribe> Tribe { get; set; }
        public sbyte Gender { get; set; }

        public Menu[] Menus { get; set; }
        public byte[] Voices { get; set; }
        public int[] FacialFeatures { get; set; }

        public RowRef<HairMakeType> FeatureMake { get; set; }

        // Decode from the API14 SDK schema, then adapt to the existing editor menus.
        public static CharaMakeType Create( ExcelPage page, uint offset, uint row ) {
            var native = new Lumina.Excel.Sheets.CharaMakeType(page, offset, row);
            var features = new int[FacialFeaturesCt];
            for (var face = 0; face < native.FacialFeatureOption.Count; face++) {
                var entry = native.FacialFeatureOption[face];
                int[] values = [entry.Option1, entry.Option2, entry.Option3, entry.Option4, entry.Option5, entry.Option6, entry.Option7];
                for (var option = 0; option < 7; option++) features[face * 7 + option] = values[option];
            }
            var menus = new Menu[MenuCt];
            for (var i = 0; i < MenuCt; i++) {
                var entry = native.CharaMakeStruct[i];
                var count = entry.SubMenuNum;
                var menu = new Menu {
                    Name = entry.Menu.IsValid ? entry.Menu.Value.Text.ExtractText() : string.Empty,
                    Default = entry.InitVal, Type = (MenuType)entry.SubMenuType, Count = count,
                    Index = (CustomizeIndex)entry.Customize, Params = new uint[count], Graphics = new byte[GraphicCt]
                };
                if (menu.HasIcon || menu.Type == MenuType.List) {
                    for (var j = 0; j < count && j < entry.SubMenuParam.Count; j++) menu.Params[j] = entry.SubMenuParam[j];
                    for (var j = 0; j < GraphicCt; j++) menu.Graphics[j] = entry.SubMenuGraphic[j];
                }
                if (menu.IsFeature) {
                    menu.Features = new RowRef<CharaMakeCustomize>[count];
                    for (var j = 0; j < count; j++) menu.Features[j] = new RowRef<CharaMakeCustomize>(page.Module, menu.Params[j], page.Language);
                }
                menus[i] = menu;
            }
            return new CharaMakeType(row) { ExcelPage = page, RowOffset = offset, Race = native.Race,
                Tribe = native.Tribe, Gender = native.Gender,
                FeatureMake = new RowRef<HairMakeType>(page.Module, row, page.Language),
                FacialFeatures = features, Voices = System.Linq.Enumerable.ToArray(native.VoiceStruct), Menus = menus };
        }
    }
}
