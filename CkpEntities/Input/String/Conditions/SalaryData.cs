namespace CkpModel.Input.String.Conditions
{
    public class SalaryData
    {
        public string Description { get; set; }
        public float? From { get; set; }
        public float? To { get; set; }
        public int? CurrencyId { get; set; }
        public bool IsSalaryPercent { get; set; }
    }
}
