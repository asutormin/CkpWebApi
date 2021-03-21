using CkpDAL.Model;
using CkpInfrastructure.Builders.Interfaces;
using CkpInfrastructure.Converters;
using CkpInfrastructure.Converters.Interfaces;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Linq;

namespace CkpServices.Helpers.Builders.AccountDocumentBuilders
{
    public class AccountDocumentPositionsBuilder : AccountDocumentBase, IBuilder<Table>
    {
        private Table _positionsTable;

        public IConverter<decimal, string> SumInWordsConverter { get; set; }
        public bool ShowDiscount { get; set; }
        public string CurrencyFormatString { get; set; }
        public Account Account { get; set; }

        public AccountDocumentPositionsBuilder()
        {
            SumInWordsConverter = new SumInWordsConverter();
            ShowDiscount = false;
            CurrencyFormatString = "#,0.00'руб.'";
        }

        protected int[] GetAccountPositionColumnWidthValues()
        {
            return
                ShowDiscount
                    ? new[] { 360, 3600, 720, 1440, 864, 1440, 1440 }
                    : new[] { 900, 4500, 1350, 1350, 1800 };
        }

        protected string[] GetAccountPositionHeaderValues()
        {
            return
                ShowDiscount
                    ? new[] { "№", "Наименование", "Кол-во", "Цена", "Скидка (%)", "Цена(с учетом скидки)", "Сумма" }
                    : new[] { "№", "Наименование", "Кол-во", "Цена", "Сумма" };
        }

        public void Build()
        {
            _positionsTable = CreateTable();
            
            FillTableHeader(_positionsTable);
            FillTableBody(_positionsTable);
            FillTableFooter(_positionsTable);
        }

        public Table GetResult()
        {
            return _positionsTable;
        }

        private Table CreateTable()
        {
            var table = new Table();

            table.AppendChild(
                new TableProperties(
                    new TableLayout { Type = TableLayoutValues.Fixed },
                    new TableWidth { Type = TableWidthUnitValues.Pct, Width = "5000" }));            

            // Устанавливаем стиль границ для столбцов таблицы позиций
            UInt32Value size = 2;
            var tableProperties = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = size },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = size },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = size },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = size },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = size },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = size }));

            table.AppendChild(tableProperties);

            return table;
        }

        private void FillTableHeader(Table table)
        {
            var widthValues = GetAccountPositionColumnWidthValues();
            var headerValues = GetAccountPositionHeaderValues();

            var headerRow = new TableRow();

            for (var i = 0; i < headerValues.Count(); i++)
            {
                var headerCell = CreateTableCell(headerValues[i], RpAccountPositionsHeader);
                headerCell.AppendChild(
                    new TableCellProperties(
                        new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = widthValues[i].ToString("D") }));

                headerRow.AppendChild(headerCell);
            }

            table.AddChild(headerRow);
        }

        private void FillTableBody(Table table)
        {
            var number = 1;
            foreach (var position in Account.AccountPositions)
            {
                var positionRow = CreatePosition(number, position);

                table.AppendChild(positionRow);

                number++;
            }
        }

        private TableRow CreatePosition(int number, AccountPosition position)
        {
            var positionRow = new TableRow();

            // Номер п.п.
            positionRow.AppendChild(
                CreateTableCell(number.ToString("D"),
                RpAccountPositions));

            // Название позиции
            positionRow.AppendChild(
                CreateTableCell(position.Name,
                RpAccountPositions,
                justification: JustificationValues.Left));

            // Кол-во
            positionRow.AppendChild(
                CreateTableCell(position.Count.ToString("D"),
                RpAccountPositions));

            // Цена
            positionRow.AppendChild(
                CreateTableCell(
                    ShowDiscount
                        ? (position.Cost / (100 - (position.Discount)) * 100).ToString("C")
                        : position.Cost.ToString(CurrencyFormatString),
                    RpAccountPositions,
                    justification: JustificationValues.Right));

            if (ShowDiscount)
            {
                // Скидка
                positionRow.AppendChild(
                    CreateTableCell(position.Discount.ToString("N"),
                    RpAccountPositions));

                // Цена (со скидкой)
                positionRow.AppendChild(
                    CreateTableCell(position.Cost.ToString(CurrencyFormatString),
                    RpAccountPositions,
                    justification: JustificationValues.Right));
            }

            // Сумма
            positionRow.AppendChild(
                CreateTableCell(position.Sum.ToString(CurrencyFormatString),
                RpAccountPositions,
                justification: JustificationValues.Right));

            return positionRow;
        }

        private void FillTableFooter(Table table)
        {
            var mergedColumns = ShowDiscount ? 5 : 3;

            #region ИТОГО

            var totalRow = new TableRow();

            var textTotal = Math.Abs(Account.Nds) < 1e-8
                ? "Итого "
                : string.Format("Итого, включая НДС : {0} % ", Account.Nds);

            AppendMergedCellsToRow(totalRow, textTotal, mergedColumns, RpAccountPositions);

            totalRow.AppendChild(
                CreateTableCell(
                    Account.Sum.ToString(CurrencyFormatString),
                    RpAccountPositions,
                    justification: JustificationValues.Right));

            table.AppendChild(totalRow);

            #endregion

            #region НДС

            var ndsRow = new TableRow();

            if (Math.Abs(Account.Nds) < 1e-8 && Account.BusinessUnitId == 3 /* ЦКП */)
            {
                AppendMergedCellsToRow(ndsRow, "Без налога (НДС)", mergedColumns, RpAccountPositions);

                ndsRow.AppendChild(
                    CreateTableCell("-", RpAccountPositions, justification: JustificationValues.Right));
            }
            else if (Math.Abs(Account.Nds) < 1e-8)
            {
                AppendMergedCellsToRow(ndsRow, "НДС не облагается в связи с применением упрощенной системы налогообложения", 
                    mergedColumns + 1, RpAccountPositionsBold, justification: JustificationValues.Center);
            }
            else
            {
                var sumNds = Account.AccountPositions.ToArray()
                    .Sum(position => position.Sum * (Account.Nds / (Account.Nds + 100)));

                AppendMergedCellsToRow(ndsRow, "В том числе НДС:", mergedColumns, RpAccountPositions);

                ndsRow.AppendChild(
                    CreateTableCell(
                        sumNds.ToString(CurrencyFormatString), RpAccountPositions, justification: JustificationValues.Right));
            }

            table.AppendChild(ndsRow);

            #endregion

            var printNewTotal = false;

            #region Счет на доплату

            if (Account.Prepaid > 0)
            {
                printNewTotal = true;

                var prepaidRow = new TableRow();

                AppendMergedCellsToRow(prepaidRow, "В авансе:", mergedColumns, RpAccountPositions);

                prepaidRow.AppendChild(
                    CreateTableCell(
                        Account.Prepaid.ToString(CurrencyFormatString),
                        RpAccountPositions, justification: JustificationValues.Right));

                table.AppendChild(prepaidRow);

                if (Math.Abs(Account.Nds) >= 1e-8)
                {
                    var prepaidNdsRow = new TableRow();

                    var prepaidNdsSum = Account.Prepaid * (1 - 100 / (100 + Account.Nds));

                    AppendMergedCellsToRow(prepaidNdsRow, "В том числе НДС:", mergedColumns, RpAccountPositions);

                    prepaidNdsRow.AppendChild(
                        CreateTableCell(
                            prepaidNdsSum.ToString(CurrencyFormatString),
                            RpAccountPositions,
                            justification: JustificationValues.Right));

                    table.AppendChild(prepaidNdsRow);
                }
            }

            #endregion

            #region Счет на оплату долга

            if (Account.Prepaid > 0)
            {
                printNewTotal = true;

                var debtRow = new TableRow();

                AppendMergedCellsToRow(debtRow, "Долг:", mergedColumns, RpAccountPositions);

                debtRow.AppendChild(
                    CreateTableCell(
                        Account.Debt.ToString(CurrencyFormatString),
                        RpAccountPositions,
                        justification: JustificationValues.Right));

                table.AppendChild(debtRow);

                if (Math.Abs(Account.Nds) >= 1e-8)
                {
                    var debtNdsRow = new TableRow();

                    var debtNdsSum = Account.Debt * (1 - 100 / (100 + Account.Nds));

                    AppendMergedCellsToRow(debtNdsRow, "В том числе НДС:", mergedColumns, RpAccountPositions);

                    debtNdsRow.AppendChild(
                      CreateTableCell(
                            debtNdsSum.ToString(CurrencyFormatString),
                            RpAccountPositions,
                            justification: JustificationValues.Right));

                    table.AppendChild(debtNdsRow);
                }
            }

            #endregion

            #region Итого к оплате

            if (printNewTotal)
            {
                var totalToPayRow = new TableRow();

                var totalToPay = Account.Sum - Account.Prepaid + Account.Debt;

                AppendMergedCellsToRow(totalToPayRow, "Итого к оплате:", mergedColumns, RpAccountPositions);

                totalToPayRow.AppendChild(
                   CreateTableCell(
                        totalToPay.ToString(CurrencyFormatString),
                        RpAccountPositions,
                        justification: JustificationValues.Right));

                table.AppendChild(totalToPayRow);
            }

            #endregion

            #region Итого прописью

            var totalToPayInWords = new TableRow();

            var totalToPayInWordsText = string.Format("Итого прописью: {0}", 
                SumInWordsConverter.Convert((decimal)(Account.Sum - Account.Prepaid + Account.Debt)));

            AppendMergedCellsToRow(totalToPayInWords, totalToPayInWordsText, mergedColumns + 1, RpAccountPositionsBoldItalic, justification: JustificationValues.Left);

            table.AppendChild(totalToPayInWords);

            #endregion
        }
    }
}
