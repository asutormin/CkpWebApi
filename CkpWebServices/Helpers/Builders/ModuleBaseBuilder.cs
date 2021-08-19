using CkpModel.Input.Module;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace CkpServices.Helpers.Builders
{
    public abstract class ModuleBaseBuilder
    {
        private readonly int _dpi;
        private readonly int _paddingPercent;

        private int _padding;
        
        protected Bitmap _bitmap;
        protected delegate void DrawCallBack(
            SolidBrush textBrush, SolidBrush backBrush, string text, Font font, StringFormat stringFormat);
        
        public ModuleParamsBaseData MaketParams { get; set; }

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

        protected void DrawMaketPart(ModulePartParamsData part, DrawCallBack drawCallBack)
        {
            var textBrush = GetBrush(part.TextColor);
            var backBrush = MaketParams.BackgroundBase64 == null
                ? part.BackgroundColor == null ? new SolidBrush(Color.White) : GetBrush(part.BackgroundColor)
                : part.BackgroundColor == null ? new SolidBrush(Color.Transparent) : GetBrush(part.BackgroundColor);

            var text = part.Text;
            var font = GetFont(part.FontFamilyName, part.FontSize, part.FontStyleName);
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
            using (var g = Graphics.FromImage(_bitmap))
            {
                var rectfOuter = new RectangleF(posX, posY, posX + width, posY + height);
                g.FillRectangle(borderBrush, rectfOuter);

                var rectfInner = new RectangleF(posX + _padding, posY + _padding, posX + width - 2 * _padding, height - 2 * _padding);
                g.FillRectangle(backBrush, rectfInner);

                g.DrawString(text, font, textBrush, rectfInner, stringFormat);
            }
        }

        private Color GetColor(ColorHolder colorHolder)
        {
            Color color = Color.FromArgb(255, colorHolder.R, colorHolder.G, colorHolder.B);

            return color;
        }

        private SolidBrush GetBrush(ColorHolder colorHolder)
        {
            var color = GetColor(colorHolder);
            var brush = new SolidBrush(color);

            return brush;
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

        private Font GetFont(string fontFamilyName, int fontSize, string fontStyleName)
        {
            var resourceManager = Properties.Resources.ResourceManager;
            var ttf = resourceManager.GetObject(string.Format("{0}_{1}", fontFamilyName, fontStyleName), Properties.Resources.Culture) as byte[];

            using (var pfc = new PrivateFontCollection())
            {
                unsafe
                {
                    fixed (Byte* pFontData = ttf)
                    {
                        pfc.AddMemoryFont((IntPtr)pFontData, ttf.Length);
                        uint InstallCount = 1;
                        AddFontMemResourceEx((IntPtr)pFontData, (uint)ttf.Length, IntPtr.Zero, ref InstallCount);
                    }
                }
                
                var font = new Font(pfc.Families[0], fontSize);

                return font;
            }
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
