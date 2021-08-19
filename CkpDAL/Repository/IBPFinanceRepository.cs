using CkpDAL.Entities;
using CkpDAL.Entities.String;
using System.Data.Common;

namespace CkpDAL.Repository
{
    public partial interface IBPFinanceRepository
    {
        string GetAccountNumber(int legalPersonId, DbTransaction dbTran);
        Account SetAccount(Account account, bool isActual, DbTransaction dbTran);
        AccountPosition SetAccountPosition(AccountPosition accountPosition, bool isActual, DbTransaction dbTran);
        AccountSettings SetAccountSettings(AccountSettings accountSettings, bool isActual, DbTransaction dbTran);
        AccountOrder SetAccountOrder(AccountOrder accountOrder, bool isActual, DbTransaction dbTran);

        Order SetOrder(Order order, bool isActual, DbTransaction dbTran);
        OrderPosition SetOrderPosition(OrderPosition order, bool isUnloaded, bool isActual, DbTransaction dbTran);
        RubricPosition SetRubricPosition(RubricPosition rubricPosition, bool isActual, DbTransaction dbTran);
        GraphicPosition SetGraphicPosition(GraphicPosition graphicPosition, bool isActual, DbTransaction dbTran);

        OrderIm SetOrderIm(OrderIm orderIm, bool isActual, DbTransaction dbTran);
        PositionIm SetPositionIm(PositionIm positionIm, bool newTaskFile, bool newMaketFile, bool isActual, DbTransaction dbTran);

        StringPosition SetString(StringPosition stringPosition, bool isActual, DbTransaction dbTran);
        Address SetAddress(Address address, bool isActual, DbTransaction dbTran);
        StringAddress SetStringAddress(StringAddress stringAddress, bool isActual, DbTransaction dbTran);
        StringOccurrence SetStringOccurrence(StringOccurrence stringOccurrence, bool isActual, DbTransaction dbTran);
        Phone SetPhone(Phone phone, bool isActual, DbTransaction dbTran);
        StringPhone SetStringPhone(StringPhone stringPhone, bool isActual, DbTransaction dbTran);
        Web SetWeb(Web web, bool isActual, DbTransaction dbTran);
        StringWeb SetStringWeb(StringWeb stringWeb, bool isActual, DbTransaction dbTran);
    }
}
