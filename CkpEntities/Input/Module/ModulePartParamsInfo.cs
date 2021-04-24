﻿
namespace CkpEntities.Input.Module
{
    public class ModulePartParamsInfo
    {
        public string FontFamilyName { get; set; }
        public int FontSize { get; set; }
        public string FontStyleName { get; set; }
        public int HorizontalAlignmentId { get; set; }
        public int VerticalAlignmentId { get; set; }
        public string Text { get; set; }
        public ColorHolder BackgroundColor { get; set; }
        public ColorHolder TextColor { get; set; }
    }
}
