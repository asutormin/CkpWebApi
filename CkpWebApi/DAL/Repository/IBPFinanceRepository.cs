using System;
using System.Data.Common;

namespace CkpWebApi.DAL.Repository
{
    public partial interface IBPFinanceRepository
    {
        void SetOrder(
            DbTransaction dbTran,
            ref int id,
            int? parentId,
            int activityTypeId,
            int businessUnitId,
            int statusId,
            bool visa,
            bool isNeedAccount,
            string accountDescription,
            string orderNumber,
            DateTime orderDate,
            DateTime? maxExitDate,
            int companyId,
            int clientLegalPersonId,
            int supplierLegalPersonId,
            float orderSum,
            float orderPaid,
            bool isCashless,
            bool isAdvance,
            bool isPaymentWithAgent,
            bool isFactoring,
            int createdPaymentPrognosisTypeId,
            int currentPaymentPrognosisTypeId,
            DateTime? paymentArbitaryPrognosisDate,
            string description,
            string request,
            int managerId,
            int editUserId,
            bool isActual,
            ref DateTime lastEditDate);

        void SetOrderPosition(
            DbTransaction dbTran,
            ref int id,
            int orderId,
            int? parentOrderPositionId,
            int supplierId,
            int priceId,
            int pricePositionId,
            DateTime pricePositionVersionDate,
            float clientDiscount,
            float markup,
            float nds,
            float compensation,
            string description,
            float clientPackageDiscount,
            int editUserId,
            bool isActual,
            ref DateTime lastEditDate,
            bool isUnloaded,
            bool needConfirmation);

        void SetRubricPosition(
            DbTransaction dbTran,
            ref int id,
            int orderPositionId,
            int rubricId,
            DateTime rubricVersionDate,
            int editUserId,
            bool isActual);

        void SetGraphicPosition(
            DbTransaction dbTran,
            ref int id,
            int parentGraphicPositionId, // если на вход приходит 0 то начало пакета, если не 0 то подчиненый
            int orderPositionId,
            int graphicId,
            int countPosition,
            int editUserId,
            bool isActual,
            ref DateTime lastEditDate);





    }
}
