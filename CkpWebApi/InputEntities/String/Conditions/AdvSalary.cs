
namespace CkpWebApi.InputEntities.String.Conditions
{
    public class AdvSalary
    {
        public string Description { get; set; }
        public float? From { get; set; }
        public float? To { get; set; }
        public int? CurrencyId { get; set; }
        public bool IsSalaryPercent { get; set; }
    }
}
