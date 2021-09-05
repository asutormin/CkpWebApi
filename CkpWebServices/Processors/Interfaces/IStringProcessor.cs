using CkpDAL.Entities;
using CkpModel.Input.String;
using CkpModel.Output.String;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace CkpServices.Processors.Interfaces
{
    interface IStringProcessor
    {
        Task<List<AddressInfo>> GetAddressesAsync(int clientLegalPersonId, string descriptionPart);
        void CreateFullString(int businessUnitId, int companyId, int orderPositionId, StringData stringData, DbTransaction dbTran);
        bool CanUpdateString(PositionIm positionIm);
        void UpdateFullString(int orderPositionId, StringData stringData, DbTransaction dbTran);
        void DeleteFullString(int orderPositionId, DbTransaction dbTran);
    }
}
