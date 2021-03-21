using CkpDAL.Model;
using CkpServices.Helpers.Builders.AccountDocumentBuilders;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CkpServices.Helpers
{
    public class AccountDocumentExporter
    {
        private readonly CultureInfo _culture = new CultureInfo("ru-RU");
        private MainDocumentPart _mainDocumentPart;

        public bool FullDetails { get; set; }
        public bool WithStamp { get; set; }
        public bool WithStampDate { get; set; }
        public Account Account { get; set; }

        public AccountDocumentDefaultsBuilder DocDefaultsBuilder { get; set; }
        public AccountDocumentPagePropertiesBuilder PagePropertiesBuilder { get; set; }
        public AccountDocumentHeaderBuilder HeaderBuilder { get; set; }
        public AccountDocumentNameBuiider NameBuilder { get; set; }
        public AccountDocumentPositionsBuilder PositionsBuilder { get; set; }
        public AccountDocumentWarningBuilder WarningBuilder { get; set; }
        public AccountDocumentSeparatorLineBuilder SeparatorLineBuilder { get; set; }
        public AccountDocumentSignsImageBuilder SignsImageBuilder { get; set; }
        public AccountDocumentSignsTextBuilder SignsTextBuilder { get; set; }
        public AccountDocumentFooterBuilder FooterBuilder { get; set; }

        public AccountDocumentExporter()
        {
            DocDefaultsBuilder = new AccountDocumentDefaultsBuilder();
            PagePropertiesBuilder = new AccountDocumentPagePropertiesBuilder();
            HeaderBuilder = new AccountDocumentHeaderBuilder();
            NameBuilder = new AccountDocumentNameBuiider();
            PositionsBuilder = new AccountDocumentPositionsBuilder();
            WarningBuilder = new AccountDocumentWarningBuilder();
            SeparatorLineBuilder = new AccountDocumentSeparatorLineBuilder();
            SignsImageBuilder = new AccountDocumentSignsImageBuilder();
            SignsTextBuilder = new AccountDocumentSignsTextBuilder();
            FooterBuilder = new AccountDocumentFooterBuilder();
        }

        public byte[] Export()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
                {
                    _mainDocumentPart = wordDocument.AddMainDocumentPart();
                    _mainDocumentPart.Document = new Document(new Body());

                    SetDocumentDefaults();
                    SetPageProperties();

                    FillHeader();
                    FillName();
                    FillPositions();
                    FillWarning();
                    for (var i = 0; i < 2; i++)
                        FillSeparatorLine();
                    FillSupplierSigns(signImageTypeId: 1);
                    FillFooter();

                    _mainDocumentPart.Document.Save();
                }

                return memoryStream.ToArray();
            }
        }

        #region Установка параметров по умолчанию для документа
        private void SetDocumentDefaults()
        {
            if (DocDefaultsBuilder == null)
                return;

            DocDefaultsBuilder.Build();

            var docDefaults = DocDefaultsBuilder.GetResult();

            var stylepart = _mainDocumentPart.AddNewPart<StyleDefinitionsPart>();
            var root = new Styles();
            root.Save(stylepart);

            _mainDocumentPart.StyleDefinitionsPart.Styles.DocDefaults = docDefaults;
        }
        #endregion

        #region Установка полей документа
        private void SetPageProperties()
        {
            if (PagePropertiesBuilder == null)
                return;

            PagePropertiesBuilder.Build();

            var pageProperties = PagePropertiesBuilder.GetResult();

            _mainDocumentPart.Document.Body.AppendChild(pageProperties);
        }
        #endregion

        #region Данные плательщика / получателя
        private void FillHeader()
        {
            if (HeaderBuilder == null)
                return;

            HeaderBuilder.Account = Account;
            HeaderBuilder.FullDetails = FullDetails;
            HeaderBuilder.Build();

            var headerTable = HeaderBuilder.GetResult();

            _mainDocumentPart.Document.Body.AppendChild(headerTable);
        }
        #endregion

        #region Заголовок (название) счёта
        private void FillName()
        {
            if (NameBuilder == null)
                return;

            var documentName = string.Format("Счет № {0} от {1}", Account.Number, Account.Date.ToShortDateString());

            NameBuilder.DocumentName = documentName;
            NameBuilder.Build();

            var nameParagraph = NameBuilder.GetResult();

            _mainDocumentPart.Document.Body.AppendChild(nameParagraph);
        }
        #endregion

        #region Позиции счёта
        private void FillPositions()
        {
            if (PositionsBuilder == null)
                return;

            PositionsBuilder.Account = Account;
            PositionsBuilder.Build();

            var positionsTable = PositionsBuilder.GetResult();

            _mainDocumentPart.Document.Body.AppendChild(positionsTable);
        }
        #endregion

        #region Предупреждение

        private void FillWarning()
        {
            if (WarningBuilder == null)
                return;

            WarningBuilder.Build();

            var warningParagraph = WarningBuilder.GetResult();

            _mainDocumentPart.Document.Body.AppendChild(warningParagraph);
        }

        #endregion

        #region Разделитель таблиц
        private void FillSeparatorLine()
        {
            if (SeparatorLineBuilder == null)
                return;

            SeparatorLineBuilder.Build();

            var separatorParagraph = SeparatorLineBuilder.GetResult();

            _mainDocumentPart.Document.Body.AppendChild(separatorParagraph);
        }
        #endregion

        #region Подписи
        private void FillSupplierSigns(int signImageTypeId)
        {
            var supplierSigns = Account.BusinessUnit.LegalPerson.LegalPersonSigns
                .FirstOrDefault(
                    s =>
                        s.ActualBegin <= Account.Date && s.ActualEnd >= Account.Date);

            if (supplierSigns == null)
                return;

            if (WithStamp)
            {
                if (SignsImageBuilder == null)
                    return;

                var signsImageInfo = supplierSigns.GetImageByMask(signImageTypeId);

                if (signsImageInfo == null)
                    return;

                Bitmap bitmap;
                using (var byteStream = new MemoryStream(signsImageInfo.SignImage))
                    bitmap = new Bitmap(byteStream);

                if (WithStampDate)
                {
                    var font = new System.Drawing.Font("Arial", 12, FontStyle.Regular);
                    var graphics = Graphics.FromImage(bitmap);
                    graphics.DrawString(Account.Date.ToString("D", _culture), font, Brushes.Black, 10, 150);
                }

                var converter = new ImageConverter();
                var imageBytes = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));

                if (imageBytes == null)
                    return;

                var imagePart = _mainDocumentPart.AddImagePart(ImagePartType.Bmp);

                using (var imageStream = new MemoryStream(imageBytes))
                    imagePart.FeedData(imageStream);

                SignsImageBuilder.RelationshipId = _mainDocumentPart.GetIdOfPart(imagePart);
                SignsImageBuilder.SignsImage = bitmap;
                SignsImageBuilder.Build();

                var drawing = SignsImageBuilder.GetResult();
                _mainDocumentPart.Document.Body.AppendChild(new Paragraph(new Run(drawing)));
            }
            else
            {
                SignsTextBuilder.Sign = supplierSigns;
                SignsTextBuilder.Build();

                var signsTable = SignsTextBuilder.GetResult();

                _mainDocumentPart.Document.Body.AppendChild(signsTable);
            }
        }
        #endregion

        #region Нижний колонтитул
        private void FillFooter()
        {
            if (FooterBuilder == null)
                return;

            FooterBuilder.Account = Account;
            FooterBuilder.Build();

            var footer = FooterBuilder.GetResult();

            var footerPart = _mainDocumentPart.AddNewPart<FooterPart>();
            footerPart.Footer = footer;

            string footerPartId = _mainDocumentPart.GetIdOfPart(footerPart);

            var sections = _mainDocumentPart.Document.Body.Elements<SectionProperties>();

            foreach (var section in sections)
            {
                section.RemoveAllChildren<FooterReference>();
                section.PrependChild(new FooterReference { Id = footerPartId });
            }
        }
        #endregion
    }
}
