using CkpWebApi.InputEntities.Module;
using System.Drawing;

namespace Helpers.Builders
{
    public class MaketStandartBuilder : ModuleBaseBuilder
    {
        public override void Build()
        {
            CreateBitmap();

            CalculatePadding();

            var maketInfo = MaketParams as ModuleParamsStandartInfo;
            /*
            var maketInfo = new ModuleParamsStandartInfo
            {
                HeightMM = 96,
                WidthMM = 128,
                Header =
                    new ModulePartParamsInfo
                    {
                        FontFamilyName = "Abbat",
                        FontSize = 20,
                        FontStyleId = (int)FontStyle.Bold,
                        HorizontalAlignmentId = (int)StringAlignment.Center,
                        VerticalAlignmentId = (int)StringAlignment.Center,
                        Text = "Header Text",
                        TextColor = new ColorHolder(r: 150, g: 200, b: 250),
                        BackgroundColor = new ColorHolder(r: 200, g: 150, b: 100)
                    },
                Body =
                    new ModulePartParamsInfo
                    {
                        FontFamilyName = "Tahoma",
                        FontSize = 18,
                        FontStyleId = (int)FontStyle.Italic,
                        HorizontalAlignmentId = (int)StringAlignment.Far,
                        VerticalAlignmentId = (int)StringAlignment.Center,
                        Text = "Body Text",
                        TextColor = new ColorHolder(r: 100, g: 150, b: 200),
                        BackgroundColor = new ColorHolder(r: 150, g: 100, b: 50)
                    },
                Footer =
                    new ModulePartParamsInfo
                    {
                        FontFamilyName = "Times New Roman",
                        FontSize = 18,
                        FontStyleId = (int)FontStyle.Underline,
                        HorizontalAlignmentId = (int)StringAlignment.Near,
                        VerticalAlignmentId = (int)StringAlignment.Far,
                        Text = "Footer Text",
                        TextColor = new ColorHolder(r: 100, g: 150, b: 50),
                        BackgroundColor = new ColorHolder(r: 150, g: 100, b: 150)
                    }
            };
            */
            DrawMaketPart(maketInfo.Header, DrawHeader);
            DrawMaketPart(maketInfo.Body, DrawBody);
            DrawMaketPart(maketInfo.Footer, DrawFooter);
        }

        private void DrawHeader(
            SolidBrush textBrush, SolidBrush backBrush,
            string text, Font font, StringFormat stringFormat)
        {
            DrawArea(
               posX: 0, posY: 0,
               width: _bitmap.Width, height: _bitmap.Height / 5,
               backBrush: Brushes.Transparent, textBrush: textBrush, borderBrush: backBrush,
               text: text, font: font, stringFormat: stringFormat);
        }

        private void DrawBody(
            SolidBrush textBrush, SolidBrush backBrush,
            string text, Font font, StringFormat stringFormat)
        {
            DrawArea(
               posX: 0, posY: _bitmap.Height / 5,
               width: _bitmap.Width, height: _bitmap.Height / 5 * 3,
               backBrush: Brushes.Transparent, textBrush: textBrush, borderBrush: backBrush,
               text: text, font: font, stringFormat: stringFormat);
        }

        private void DrawFooter(
            SolidBrush textBrush, SolidBrush backBrush,
            string text, Font font, StringFormat stringFormat)
        {
            DrawArea(
               posX: 0, posY: _bitmap.Height / 5 * 4,
               width: _bitmap.Width, height: _bitmap.Height / 5,
               backBrush: Brushes.Transparent, textBrush: textBrush, borderBrush: backBrush,
               text: text, font: font, stringFormat: stringFormat);
        }
    }
}
