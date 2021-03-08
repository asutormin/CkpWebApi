using CkpWebApi.Infrastructure.Interfaces;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CkpWebApi.Helpers.Builders.AccountDocumentBuilders
{
    public class AccountDocumentWarningBuilder : AccountDocumentBase, IBuilder<Paragraph>
    {
        private Paragraph _warningParagraph;

        public void Build()
        {
            const string warning = "Внимание! В платежном поручении сумма должна строго соответствовать выписанному счету, " +
                                   "указание номера счета в поле \"Назначение платежа\" обязательно.";

            _warningParagraph =
               new Paragraph(
                    new ParagraphProperties(
                        new SpacingBetweenLines { Before = "210", After = "210" },
                        new Justification { Val = JustificationValues.Left }),
                    new TabChar(),
                    new Run(
                        RpText.CloneNode(true),
                        new Text { Text = warning, Space = SpaceProcessingModeValues.Preserve }));
        }

        public Paragraph GetResult()
        {
            return _warningParagraph;
        }

    }
}
