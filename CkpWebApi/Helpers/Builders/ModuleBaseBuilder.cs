using CkpWebApi.InputEntities.Module;
using System;
using System.Drawing;

namespace Helpers.Builders
{
    public abstract class ModuleBaseBuilder
    {
        private readonly int _dpi;
        private readonly int _paddingPercent;

        private int _padding;
        
        protected Bitmap _bitmap;
        protected delegate void DrawCallBack(
            SolidBrush textBrush, SolidBrush backBrush, string text, Font font, StringFormat stringFormat);
        
        public ModuleParamsBaseInfo MaketParams { get; set; }

        public ModuleBaseBuilder()
        {
            _dpi = 300;
            _paddingPercent = 2;
        }

        public abstract void Build();

        public Bitmap GetResult()
        {
            return _bitmap;
        }

        protected void CreateBitmap()
        {
            var ptWidth = MaketParams.WidthMM / 25.4 * _dpi;
            var ptHeight = MaketParams.HeightMM / 25.4 * _dpi;

            _bitmap = new Bitmap((int)ptWidth, (int)ptHeight);
            _bitmap.SetResolution(_dpi, _dpi);
        }

        protected void CalculatePadding()
        {
            _padding = Math.Min(_bitmap.Width, _bitmap.Height) * _paddingPercent / 100;
        }

        protected void DrawMaketPart(ModulePartParamsInfo part, DrawCallBack drawCallBack)
        {
            var textBrush = GetBrush(part.TextColor);
            var backBrush = GetBrush(part.BackgroundColor);

            var text = part.Text;
            var font = GetFont(part.FontFamilyName, part.FontSize, part.FontStyleId);
            var stringFormat = GetStringFormat(part.HorizontalAlignmentId, part.VerticalAlignmentId);

            drawCallBack(
                textBrush: textBrush, backBrush: backBrush,
                text: text, font: font, stringFormat: stringFormat);
        }

        protected void DrawArea(
            int posX, int posY,
            int width, int height,
            Brush backBrush, Brush textBrush, Brush borderBrush,
            string text, Font font, StringFormat stringFormat)
        {
            var g = Graphics.FromImage(_bitmap);

            var rectfOuter = new RectangleF(posX, posY, posX + width, posY + height);
            g.FillRectangle(borderBrush, rectfOuter);

            var rectfInner = new RectangleF(posX + _padding, posY + _padding, posX + width - 2 * _padding, height - 2 * _padding);
            g.FillRectangle(backBrush, rectfInner);

            g.DrawString(text, font, textBrush, rectfInner, stringFormat);
        }

        private Color GetColor(ColorHolder colorHolder)
        {
            var color = Color.FromArgb(255, colorHolder.R, colorHolder.G, colorHolder.B);

            return color;
        }

        private SolidBrush GetBrush(ColorHolder colorHolder)
        {
            var color = GetColor(colorHolder);
            var brush = new SolidBrush(color);

            return brush;
        }

        private Font GetFont(string fontFamilyName, int fontSize, int fontStyleId)
        {
            var fontStyle = (FontStyle)fontStyleId;
            var font = new Font(fontFamilyName, fontSize, fontStyle);

            return font;
        }

        private StringFormat GetStringFormat(int horizontalAlignmentId, int verticalAlignmentId)
        {
            var stringFormat = new StringFormat
            {
                Alignment = (StringAlignment)horizontalAlignmentId,
                LineAlignment = (StringAlignment)verticalAlignmentId,
                Trimming = StringTrimming.Word
            };

            return stringFormat;
        }
    }
}
