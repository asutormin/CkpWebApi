using CkpWebApi.Infrastructure.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CkpWebApi.Helpers.Builders.AccountDocumentBuilders
{
    public class AccountDocumentPagePropertiesBuilder : IBuilder<SectionProperties>
    {
        private SectionProperties _sectionProperties;

        public void Build()
        {
            _sectionProperties = 
                new SectionProperties(
                    new PageSize
                    {
                        Width = 11906,
                        Height = 16838
                    },
                    new PageMargin
                    {
                        Top = 567,
                        Right = 1134,
                        Bottom = 567,
                        Left = 1134,
                        Header = 720,
                        Footer = 720,
                        Gutter = 0
                    });
        }

        public SectionProperties GetResult()
        {
            return _sectionProperties;
        }
    }
}
