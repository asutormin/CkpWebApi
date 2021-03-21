using CkpInfrastructure.Builders.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CkpServices.Helpers.Builders.AccountDocumentBuilders
{
    public class AccountDocumentSeparatorLineBuilder : AccountDocumentBase, IBuilder<Paragraph>
    {
        private Paragraph _separatorParagraph;

        public void Build()
        {
            _separatorParagraph =
                new Paragraph(
                    new Run(
                        RpSeparator.CloneNode(true)));
        }

        public Paragraph GetResult()
        {
            return _separatorParagraph;
        }
    }
}
