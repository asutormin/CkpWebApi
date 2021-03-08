using CkpWebApi.DAL.Model;
using CkpWebApi.Infrastructure.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;

namespace CkpWebApi.Helpers.Builders.AccountDocumentBuilders
{
    public class AccountDocumentFooterBuilder : AccountDocumentBase, IBuilder<Footer>
    {
        private Footer _footer;

        public Account Account { get; set; }

        public void Build()
        {
            var accountOrder = Account.AccountOrders.FirstOrDefault();

            var managerText = string.Format("Менеджер: {0}", accountOrder == null ? string.Empty : accountOrder.Order.Manager.Name);
            var ordersText = string.Format("Заказы: {0}", string.Join(", ", Account.AccountOrders.Select(ao => ao.OrderId)));

            _footer = new Footer(
                new Paragraph(
                   new ParagraphProperties(
                       new Justification { Val = JustificationValues.Right },
                       new SpacingBetweenLines { Before = "0", After = "0" }),
                   new Run(
                       RpFooter.CloneNode(true),
                       new Text(managerText))),
                new Paragraph(
                   new ParagraphProperties(
                       new Justification { Val = JustificationValues.Right },
                       new SpacingBetweenLines { Before = "0", After = "0" }),
                   new Run(
                        RpFooter.CloneNode(true),
                       new Text(ordersText)))
                );

            if (!string.IsNullOrEmpty(Account.Request))
            {
                _footer.AppendChild(new Paragraph(new ParagraphProperties(
                                   new Justification { Val = JustificationValues.Right },
                                   new SpacingBetweenLines { Before = "0", After = "0" }),
                               new Run(
                                    RpFooter.CloneNode(true),
                                   new Text(Account.Request))));
            }
        }

        public Footer GetResult()
        {
            return _footer;
        }
    }
}
