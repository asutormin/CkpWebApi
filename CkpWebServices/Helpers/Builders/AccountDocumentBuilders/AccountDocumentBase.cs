using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Drawing;
using System.Globalization;

namespace CkpServices.Helpers.Builders.AccountDocumentBuilders
{
    /// <summary>
    /// Базовый класс для печати счета.
    /// </summary>
    public abstract class AccountDocumentBase
    {
        protected CultureInfo Culture = new CultureInfo("ru-RU");

        #region Шрифты

        // Заголовок (название) документа
        protected RunProperties RpHeader = new RunProperties(
            new RunFonts { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
            new FontSize { Val = "28" },
            new Bold { Val = OnOffValue.FromBoolean(true) },
            new Justification { Val = JustificationValues.Center });

        // Данные плательщика / получателя
        protected RunProperties RpDetails = new RunProperties(
            new RunFonts { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
            new FontSize { Val = "20" });
        protected RunProperties RpDetailsBold = new RunProperties(
            new RunFonts { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
            new FontSize { Val = "20" },
            new Bold { Val = OnOffValue.FromBoolean(true) });

        // Текстовая информация (предупреждения)
        protected RunProperties RpText = new RunProperties(
            new RunFonts { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
            new FontSize { Val = "20" });

        // Позиции счета
        protected RunProperties RpAccountPositionsHeader = new RunProperties(
            new RunFonts { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
            new FontSize { Val = "20" },
            new Bold { Val = OnOffValue.FromBoolean(true) });
        protected RunProperties RpAccountPositions = new RunProperties(
            new RunFonts { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
            new FontSize { Val = "18" });
        protected RunProperties RpAccountPositionsBold = new RunProperties(
            new RunFonts { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
            new FontSize { Val = "18" },
            new Bold { Val = OnOffValue.FromBoolean(true) });
        protected RunProperties RpAccountPositionsItalic = new RunProperties(
            new RunFonts { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
            new FontSize { Val = "18" },
            new Italic { Val = OnOffValue.FromBoolean(true) });
        protected RunProperties RpAccountPositionsBoldItalic = new RunProperties(
            new RunFonts { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
            new FontSize { Val = "18" },
            new Bold { Val = OnOffValue.FromBoolean(true) },
            new Italic { Val = OnOffValue.FromBoolean(true) });

        // Подписи
        protected RunProperties RpSigns = new RunProperties(
            new RunFonts { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
            new FontSize { Val = "20" });

        // Заказ, исполнитель
        protected RunProperties RpFooter = new RunProperties(
            new RunFonts { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
            new FontSize { Val = "14" });

        // Разделитель (отделяет таблицы друг от друга)
        protected RunProperties RpSeparator = new RunProperties(
            new RunFonts { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
            new FontSize { Val = "14" });

        #endregion

        #region Работа с ячейками таблицы

        /// <summary>
        /// Создаёт ячейку таблицы.
        /// </summary>
        /// <param name="runProperties">Параметры текста.</param>
        /// <param name="aligment">Вертикальное выравнивание.</param>
        /// <param name="justification">Горизонтальное выравнивание.</param>
        /// <param name="text">Текст в ячейке.</param>
        /// <returns>Ячеёка таблицы.</returns>
        protected static TableCell CreateTableCell(
            string text,
            RunProperties runProperties,
            TableVerticalAlignmentValues aligment = TableVerticalAlignmentValues.Center,
            JustificationValues justification = JustificationValues.Center)
        {
            var textLines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            var tableCell = new TableCell(
                new TableCellProperties(new TableCellVerticalAlignment { Val = aligment }));

            foreach (var line in textLines)
            {
                tableCell.AppendChild(
                    new Paragraph(
                        new ParagraphProperties(
                            new Justification { Val = justification },
                            new SpacingBetweenLines { Before = "0", After = "0" }),
                            new Run(
                                runProperties.CloneNode(true),
                                new Text
                                {
                                    Text = line,
                                    Space = SpaceProcessingModeValues.Preserve
                                })));
            }

            return tableCell;
        }

        /// <summary>
        /// Создаёт ячейку таблицы, предназначенную для объединения.
        /// </summary>
        /// <param name="text">Текст в ячейке.</param>
        /// <param name="isFirstCellInMerge">Ячейка является первой по порядку среди объединяемых ячеек.</param>
        /// <param name="runProperties">Параметры текста.</param>
        /// <param name="aligment">Вертикальное выравнивание.</param>
        /// <param name="justification">Горизонтальное выравнивание.</param>
        /// <returns>Ячейка таблицы для объединения.</returns>
        private static TableCell CreateTableCellForMerging(
            string text,
            bool isFirstCellInMerge,
            RunProperties runProperties,
            TableVerticalAlignmentValues aligment,
            JustificationValues justification)
        {
            var cellProperties = new TableCellProperties(
                    new HorizontalMerge
                    {
                        Val =
                            isFirstCellInMerge
                            ? MergedCellValues.Restart
                            : MergedCellValues.Continue
                    });

            var cellForMerging = CreateTableCell(text, runProperties, aligment, justification);

            cellForMerging.AppendChild(cellProperties);

            return cellForMerging;
        }

        /// <summary>
        /// Добавляет к строке таблицы объединённые ячейки.
        /// </summary>
        /// <param name="tableRow">Строка таблицы.</param>
        /// <param name="text">Текст в объединённой ячейки.</param>
        /// <param name="mergedCells">Количество объединяемых ячеек.</param>
        /// <param name="runProperties">Параметры текста.</param>
        /// <param name="aligment">Вертикальное выравнивание.</param>
        /// <param name="justification">Горизонтальное выравнивание.</param>
        protected void AppendMergedCellsToRow(
            TableRow tableRow,
            string text,
            int mergedCells,
            RunProperties runProperties,
            TableVerticalAlignmentValues aligment = TableVerticalAlignmentValues.Center,
            JustificationValues justification = JustificationValues.Right)
        {
            for (var i = 0; i <= mergedCells; i++)
            {
                tableRow.AppendChild(
                    i == 0
                        ? CreateTableCellForMerging(text, true, runProperties, aligment, justification)
                        : CreateTableCellForMerging(string.Empty, false, runProperties, aligment, justification));
            }
        }

        #endregion

        #region Работа с изображениями

        /// <summary>
        /// Пересчёт размеров изображения для вывода в документ.
        /// </summary>
        /// <param name="image">Объект изображения.</param>
        /// <param name="widthEmus">Пересчитанная ширина.</param>
        /// <param name="heightEmus">Пересчитанная высота.</param>
        protected static void CalculateImageEmusSize(Image image, out long widthEmus, out long heightEmus)
        {
            var widthPx = image.Width;
            var heightPx = image.Height;

            var horzRezDpi = image.HorizontalResolution;
            var vertRezDpi = image.VerticalResolution;

            const int emusPerInch = 914400;
            const int emusPerCm = 360000;

            widthEmus = (long)(widthPx / horzRezDpi * emusPerInch);
            heightEmus = (long)(heightPx / vertRezDpi * emusPerInch);

            const long maxWidthEmus = (long)(19 * emusPerCm);

            if (widthEmus <= maxWidthEmus) return;

            var ratio = (heightEmus * 1.0m) / widthEmus;
            widthEmus = maxWidthEmus;
            heightEmus = (long)(widthEmus * ratio);
        }

        /// <summary>
        /// Добавление изображения в тело документа.
        /// </summary>
        /// <param name="image">Изобрадение.</param>
        /// <param name="docBody">Ссылка на тело документа.</param>
        /// <param name="relationshipId">Идентификатор части документа с изображением.</param>
        /// <param name="multiplier">Коэффициент масштабирования изображения.</param>
        protected static void AddImageToBody(Image image, Body docBody, string relationshipId, double multiplier)
        {
            long widthEmus, heightEmus;
            CalculateImageEmusSize(image, out widthEmus, out heightEmus);

            widthEmus = (long)(widthEmus * multiplier);
            heightEmus = (long)(heightEmus * multiplier);

            var element =
                 new Drawing(
                     new DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline(
                         new DocumentFormat.OpenXml.Drawing.Wordprocessing.Extent { Cx = widthEmus, Cy = heightEmus },
                         new DocumentFormat.OpenXml.Drawing.Wordprocessing.EffectExtent
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DocumentFormat.OpenXml.Drawing.Wordprocessing.DocProperties { Id = (UInt32Value)1U, Name = "Picture 1" },
                         new DocumentFormat.OpenXml.Drawing.Wordprocessing.NonVisualGraphicFrameDrawingProperties(
                             new DocumentFormat.OpenXml.Drawing.GraphicFrameLocks { NoChangeAspect = true }),
                         new DocumentFormat.OpenXml.Drawing.Graphic(
                             new DocumentFormat.OpenXml.Drawing.GraphicData(
                                 new DocumentFormat.OpenXml.Drawing.Pictures.Picture(
                                     new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureProperties(
                                         new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualDrawingProperties { Id = (UInt32Value)0U, Name = "New Bitmap Image.jpg" },
                                         new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureDrawingProperties()),
                                     new DocumentFormat.OpenXml.Drawing.Pictures.BlipFill(
                                         new DocumentFormat.OpenXml.Drawing.Blip(
                                             new DocumentFormat.OpenXml.Drawing.BlipExtensionList(
                                                 new DocumentFormat.OpenXml.Drawing.BlipExtension { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" }))
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             DocumentFormat.OpenXml.Drawing.BlipCompressionValues.Print
                                         },
                                         new DocumentFormat.OpenXml.Drawing.Stretch(
                                             new DocumentFormat.OpenXml.Drawing.FillRectangle())),
                                     new DocumentFormat.OpenXml.Drawing.Pictures.ShapeProperties(
                                         new DocumentFormat.OpenXml.Drawing.Transform2D(
                                             new DocumentFormat.OpenXml.Drawing.Offset { X = 0L, Y = 0L },
                                             new DocumentFormat.OpenXml.Drawing.Extents { Cx = widthEmus, Cy = heightEmus }
                                             ),
                                         new DocumentFormat.OpenXml.Drawing.PresetGeometry(
                                             new DocumentFormat.OpenXml.Drawing.AdjustValueList()
                                         )
                                         { Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U//,
                         //EditId = "50D07946" // Не поддерживается в MS Office 2007
                     });

            docBody.AppendChild(new Paragraph(new Run(element)));
        }

        #endregion
    }
}
