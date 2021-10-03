using CkpModel.Output;
using System.Collections.Generic;

namespace CkpServices.Interfaces
{
    public interface ISearchService
    {
        List<OrderPositionInfo> Search(int clientLegalPersonId, string value, int skipCount);
    }
}
