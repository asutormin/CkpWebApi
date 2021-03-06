using System;

namespace CkpModel.Output
{
    public class AccountInfoLight
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public LegalPersonInfo SupplierLegalPerson { get; set; }
        public float Nds { get; set; }
        public float Sum { get; set; }
        public float Debt { get; set; }
        public int TypeId { get; set; }
    }
}
