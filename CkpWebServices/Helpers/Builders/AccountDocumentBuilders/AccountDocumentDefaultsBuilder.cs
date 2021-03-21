using CkpInfrastructure.Builders.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CkpServices.Helpers.Builders.AccountDocumentBuilders
{
    public class AccountDocumentDefaultsBuilder : IBuilder<DocDefaults>
    {
        private DocDefaults _docDefaults;

        public void Build()
        {
            _docDefaults = new DocDefaults(
                new ParagraphPropertiesDefault(
                    new ParagraphProperties(
                        new SpacingBetweenLines { Before = "0", After = "0" })),
                new RunPropertiesDefault(
                    new RunProperties(
                        new RunFonts { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" },
                        new FontSize { Val = "16" })));
        }

        public DocDefaults GetResult()
        {
            return _docDefaults;
        }
    }
}
