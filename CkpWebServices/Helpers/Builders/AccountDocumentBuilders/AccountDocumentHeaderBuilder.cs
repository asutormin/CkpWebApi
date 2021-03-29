using CkpDAL.Model;
using CkpInfrastructure.Builders.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;
using System.Text;

namespace CkpServices.Helpers.Builders.AccountDocumentBuilders
{
    public class AccountDocumentHeaderBuilder : AccountDocumentBase, IBuilder<Table>
    {
        private Table _headerTable;

        public bool FullDetails { get; set; }
        public Account Account { get; set; }

        public void Build()
        {
            _headerTable = new Table(
                new TableProperties(
                    new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Top },
                    new TableJustification { Val = TableRowAlignmentValues.Left },
                    new TableWidth { Type = TableWidthUnitValues.Pct, Width = "5000" }));

            #region Ячейки получателя

            // Получатель заголовок
            var recipientHeaderCell = CreateDetailsCell(RpDetailsBold, "Получатель");

            var supplierLegalPerson = Account.BusinessUnit.LegalPerson;

            // Наименование получателя
            var recipientNameCell = CreateDetailsCell(RpDetailsBold, supplierLegalPerson.Name);

            // Адрес
            var recipientAddress = string.Format("Адрес: {0}", supplierLegalPerson.LegalAddress);
            var recipientAddressCell = CreateDetailsCell(RpDetails, recipientAddress);

            // ИНН/КПП
            var recipientInnKpp = string.Format("ИНН/КПП {0}/{1}", supplierLegalPerson.Inn, supplierLegalPerson.Kpp);
            var recipientInnKppCell = CreateDetailsCell(RpDetails, recipientInnKpp);

            var supplierBank = GetSupplierBank(Account);

            // Расчетный счет
            var recipientSettlementAccount = string.Format("Расчетный счет: {0}", GetSupplierSettlementAccount(Account));
            var recipientSettlementAccountCell = CreateDetailsCell(RpDetailsBold, recipientSettlementAccount,  supplierBank == null ? string.Empty : supplierBank.Name);

            // БИК
            var recipientBankBik = string.Format("БИК {0}", supplierBank == null ? string.Empty : supplierBank.Bik);
            var recipientBankBikCell = CreateDetailsCell(RpDetailsBold, recipientBankBik);

            // К/с
            var recipientBankCorrespondentAccount = string.Format("К/с {0}", supplierBank == null ? string.Empty : supplierBank.CorrespondentAccount);
            var recipientBankCorrespondentAccountCell = CreateDetailsCell(RpDetailsBold, recipientBankCorrespondentAccount);

            // Доп. сведения
            var recipientAdditionalDescription = string.Format("Доп. сведения: {0}",
                supplierLegalPerson.AccountSettings == null ? string.Empty : supplierLegalPerson.AccountSettings.AdditionalDescription);
            var recipientAdditionalDescriptionCell = CreateDetailsCell(RpDetailsBold, recipientAdditionalDescription);

            #endregion

            #region Ячейки плательщика

            // Плательщик заголовок
            var payerHeaderCell = CreateDetailsCell(RpDetailsBold, "Плательщик");

            var clientLegalPerson = Account.LegalPerson;

            // Наименование плательщика
            var payerNameCell = CreateDetailsCell(RpDetails, clientLegalPerson.Name);

            // Адрес
            var payerAddress = string.Format("Адрес: {0}", clientLegalPerson.LegalAddress);
            var payerAddressCell = CreateDetailsCell(RpDetails, payerAddress);

            // ИНН/КПП
            var payerInnKpp = string.Format("ИНН/КПП {0}/{1}",
                clientLegalPerson.Inn == null ? string.Empty : clientLegalPerson.Inn, 
                clientLegalPerson.Kpp == null ? string.Empty : clientLegalPerson.Kpp);
            var payerInnKppCell = CreateDetailsCell(RpDetails, payerInnKpp);

            var clientBank = GetClientBank(Account);

            // Расчетный счет
            var payerSettlementAccount = string.Format("Расчетный счет: {0}", GetClientSettlementAccount(clientLegalPerson, Account.AccountSettings));
            var payerSettlementAccountCell = CreateDetailsCell(RpDetails, payerSettlementAccount, clientBank == null ? string.Empty : clientBank.Name);

            // БИК
            var payerBankBik = string.Format("БИК {0}", clientBank == null ? string.Empty : clientBank.Bik);
            var payerBankBikCell = CreateDetailsCell(RpDetails, payerBankBik);

            // К/с
            var payerBankCorrespondentAccount = string.Format("К/с {0}", clientBank == null ? string.Empty : clientBank.CorrespondentAccount);
            var payerBankCorrespondentAccountCell = CreateDetailsCell(RpDetails, payerBankCorrespondentAccount);

            // Доп. сведения
            var sb = new StringBuilder();
            if (Account.AccountSettings.ShowOkpo)
            {
                sb.AppendFormat("ОКПО {0}", clientLegalPerson.Okpo);
            }
            if (Account.AccountSettings.ShowContractInCaption)
            {
                if (sb.Length > 0)
                    sb.AppendLine();
                sb.AppendFormat("Договор №{0} от {1}",
                    Account.AccountSettings.ContractNumber, Account.AccountSettings.ContractDate);
            }
            if (Account.AccountSettings.ShowAdditionalDescription)
            {
                if (sb.Length > 0)
                    sb.AppendLine();
                sb.AppendFormat("Доп. сведения: {0}",
                    Account.AccountSettings.AdditionalDescription);
            }

            var payerAdditionalDescriptionCell = CreateDetailsCell(RpDetails, sb.ToString());

            var emtyCell = CreateDetailsCell(RpDetails, string.Empty);

            #endregion

            #region Добавление ячеек в таблицу

            _headerTable.AppendChild(
                new TableRow(recipientHeaderCell, payerHeaderCell));

            _headerTable.AppendChild(
                new TableRow(recipientNameCell, payerNameCell));

            _headerTable.AppendChild(
                new TableRow(emtyCell.CloneNode(true), emtyCell.CloneNode(true)));

            _headerTable.AppendChild(
                new TableRow(recipientAddressCell, payerAddressCell));

            _headerTable.AppendChild(
                new TableRow(recipientInnKppCell, payerInnKppCell));

            _headerTable.AppendChild(
                new TableRow(recipientSettlementAccountCell, FullDetails ? payerSettlementAccountCell : emtyCell.CloneNode(true)));

            _headerTable.AppendChild(
                new TableRow(recipientBankBikCell, FullDetails ? payerBankBikCell : emtyCell.CloneNode(true)));

            _headerTable.AppendChild(
                new TableRow(recipientBankCorrespondentAccountCell, FullDetails ? payerBankCorrespondentAccountCell : emtyCell.CloneNode(true)));

            _headerTable.AppendChild(
                new TableRow(
                    supplierLegalPerson.AccountSettings.ShowAdditionalDescription
                        ? recipientAdditionalDescriptionCell
                        : emtyCell.CloneNode(true),
                    payerAdditionalDescriptionCell));

            #endregion
        }

        public Table GetResult()
        {
            return _headerTable;
        }

        private string GetSupplierSettlementAccount(Account account)
        {
            string settlementAccount = string.Empty;

            if (account == null)
                return settlementAccount;

            var businessUnit = account.BusinessUnit;
            if (businessUnit == null)
                return settlementAccount;

            var cash = businessUnit.Cash;
            if (cash == null)
                return settlementAccount;

            var legalPersonBank = cash.LegalPersonBank;
            if (legalPersonBank == null)
                return settlementAccount;

            if (legalPersonBank.SettlementAccount != null)
                settlementAccount = legalPersonBank.SettlementAccount;

            return settlementAccount;
        }

        private string GetClientSettlementAccount(LegalPerson legalPerson, AccountSettings accountSettings)
        {
            string settlementAccount = string.Empty;

            if (accountSettings != null)
            {
                var legalPersonBank = legalPerson.LegalPersonBanks
                    .FirstOrDefault(lpb => lpb.Id == accountSettings.LegalPersonBankId);

                if (legalPersonBank != null)
                    settlementAccount = legalPersonBank.SettlementAccount;
            }

            return settlementAccount;
        }

        private Bank GetSupplierBank(Account account)
        {
            if (account == null)
                return null;

            var businessUnit = account.BusinessUnit; 
            if (businessUnit == null)
                return null;

            var cash = businessUnit.Cash;
            if (cash == null)
                return null;

            var legalPersonBank = cash.LegalPersonBank;
            if (legalPersonBank == null)
                return null;

            return legalPersonBank.Bank;
        }

        private Bank GetClientBank(Account account)
        {
            if (account == null)
                return null;

            var accountSettings = account.AccountSettings;
            if (accountSettings == null)
                return null;

            var legalPersonBank = accountSettings.LegalPersonBank;
            if (legalPersonBank == null)
                return null;

            return legalPersonBank.Bank;
        }

        #region Ячейка

        protected virtual TableCell CreateDetailsCell(RunProperties runProperties, params string[] cellText)
        {
            var cell = new TableCell(
                new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "2500" });

            foreach (var ct in cellText)
            {
                var paragraph = new Paragraph(
                    new ParagraphProperties(
                        new SpacingBetweenLines { Before = "0", After = "0" }),
                    new Run(
                        runProperties.CloneNode(true),
                        new Text(ct)));

                cell.AppendChild(paragraph);
            }

            return cell;
        }

        #endregion
    }
}
