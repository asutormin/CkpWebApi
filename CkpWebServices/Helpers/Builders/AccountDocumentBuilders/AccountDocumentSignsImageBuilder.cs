using CkpInfrastructure.Builders.Interfaces;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Drawing;

namespace CkpServices.Helpers.Builders.AccountDocumentBuilders
{
    public class AccountDocumentSignsImageBuilder : AccountDocumentBase, IBuilder<Drawing>
    {
        private Drawing _signsDrawing;

        public string RelationshipId { get; set; }
        public double Multiplier { get; set; }
        public Image SignsImage { get; set; }

        public AccountDocumentSignsImageBuilder()
        {
            Multiplier = 0.75; // Коэффициент масштабирования изображения
        }

        public void Build()
        {
            long widthEmus, heightEmus;
            CalculateImageEmusSize(SignsImage, out widthEmus, out heightEmus);

            widthEmus = (long)(widthEmus * Multiplier);
            heightEmus = (long)(heightEmus * Multiplier);

            _signsDrawing =
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
                                             Embed = RelationshipId,
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
        }

        public Drawing GetResult()
        {
            return _signsDrawing;
        }
    }
}
