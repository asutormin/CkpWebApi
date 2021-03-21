using CkpDAL.Model;
using CkpInfrastructure.Builders.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;

namespace CkpServices.Helpers.Builders.AccountDocumentBuilders
{
    public class AccountDocumentSignsTextBuilder : AccountDocumentBase, IBuilder<Table>
    {
        private Table _signsTable;

        public LegalPersonSign Sign { get; set;}

        public void Build()
        {
            _signsTable = new Table(
                new TableProperties(
                    new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center },
                    new TableLayout { Type = TableLayoutValues.Fixed },
                    new TableWidth { Type = TableWidthUnitValues.Pct, Width = "5000" }));

            var row1 = new TableRow();

            var topCell1 = CreateTableCell(string.Empty, RpSigns);

            topCell1.AppendChild(
                new TableCellProperties(
                    new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "1000" }));

            var topCell2 = CreateTableCell(string.Empty, RpSigns);

            topCell2.AppendChild(
                new TableCellProperties(
                    new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "1000" }));

            var topCell3 = CreateTableCell(string.Empty, RpSigns);

            topCell3.AppendChild(
                new TableCellProperties(
                    new TableCellWidth { Type = TableWidthUnitValues.Pct, Width = "3000" }));

            row1.Append(topCell1, topCell2, topCell3);

            _signsTable.AppendChild(row1);

            var row2 = new TableRow();

            row2.Append(
                CreateTableCell(string.Empty, RpSigns),
                CreateTableCell(string.Empty, RpSigns),
                CreateTableCell(string.Empty, RpSigns));

            _signsTable.AppendChild(row2);

            var row3 = new TableRow();

            row3.Append(
                CreateTableCell(string.Empty, RpSigns),
                CreateTableCell("Руководитель", RpSigns, aligment: TableVerticalAlignmentValues.Top, justification: JustificationValues.Right),
                CreateTableCell(string.Format("___________________________\\{0}\\", Sign.DirectorSignText), RpSigns,
                    TableVerticalAlignmentValues.Bottom, JustificationValues.Left));

            _signsTable.AppendChild(row3);

            var row4 = new TableRow();

            row4.Append(
                CreateTableCell(string.Empty, RpSigns),
                CreateTableCell(string.Empty, RpSigns),
                CreateTableCell(string.Empty, RpSigns));

            _signsTable.AppendChild(row4);

            var row5 = new TableRow();

            row5.Append(
                CreateTableCell(string.Empty, RpSigns),
                CreateTableCell(string.Empty, RpSigns),
                CreateTableCell(string.Empty, RpSigns));

            _signsTable.AppendChild(row5);

            var row6 = new TableRow();

            row6.Append(
                CreateTableCell(string.Empty, RpSigns),
                CreateTableCell("Главный бухгалтер", RpSigns, justification: JustificationValues.Right),
                CreateTableCell(string.Format("___________________________\\{0}\\", Sign.AccountantSignText), RpSigns,
                    justification: JustificationValues.Left));
            _signsTable.AppendChild(row6);
        }

        public Table GetResult()
        {
            return _signsTable;
        }
    }
}
