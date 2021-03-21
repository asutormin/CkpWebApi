using System.Collections.Generic;

namespace CkpEntities.Output
{
    public class AccountInfo : AccountLight
    {
        public LegalPersonInfo ClientLegalPerson { get; set; }
        public LegalPersonBankInfo Bank { get; set; }
        public IEnumerable<PositionInfo> Positions { get; set; }
    }
}
