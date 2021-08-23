using System.Collections.Generic;

namespace CkpModel.Output
{
    public class AccountInfo : AccountInfoLight
    {
        public LegalPersonInfo ClientLegalPerson { get; set; }
        public LegalPersonBankInfo Bank { get; set; }
        public IEnumerable<OrderPositionInfo> OrderPositions { get; set; }
        public IEnumerable<AccountPositionInfo> AccountPositions { get; set; }
    }
}
