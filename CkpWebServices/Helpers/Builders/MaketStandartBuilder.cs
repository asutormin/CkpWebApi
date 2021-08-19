using CkpModel.Input.Module;
using CkpServices.Helpers.Converters;
using System;
using System.Drawing;
using System.IO;

namespace CkpServices.Helpers.Builders
{
    public class MaketStandartBuilder : ModuleBaseBuilder
    {
        public override void Build()
        {
            CreateBitmap();

            CalculatePadding();

            var maketInfo = MaketParams as ModuleParamsStandartData;

            if (maketInfo.BackgroundBase64 != null)
                DrawBackground(maketInfo.BackgroundBase64);

            DrawMaketPart(maketInfo.Header, DrawHeader);
            DrawMaketPart(maketInfo.Body, DrawBody);
            DrawMaketPart(maketInfo.Footer, DrawFooter);
        }

        private void DrawBackground(string base64)
        {
            var bytes = Base64ToBytesConverter.Convert(base64);

            using (var stream = new MemoryStream(bytes))
            {
                var image = Image.FromStream(stream);

                using (var g = Graphics.FromImage(_bitmap))
                {
                    g.DrawImage(image, 0, 0, _bitmap.Width, _bitmap.Height);
                }
            }
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
