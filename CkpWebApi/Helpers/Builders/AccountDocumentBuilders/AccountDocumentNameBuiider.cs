using CkpWebApi.Infrastructure.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CkpWebApi.Helpers.Builders.AccountDocumentBuilders
{
    public class AccountDocumentNameBuiider : AccountDocumentBase, IBuilder<Paragraph>
    {
        private Paragraph _nameParagraph;

        public string DocumentName { get; set; }

        public void Build()
        {
            _nameParagraph =
                new Paragraph(
                    new ParagraphProperties(
                        new SpacingBetweenLines { Before = "210", After = "290" },
                        new Justification { Val = JustificationValues.Center }),
                    new Run(
                        RpHeader.CloneNode(true),
                        new Text(DocumentName)));
        }

        public Paragraph GetResult()
        {
            return _nameParagraph;
        }
    }
}
