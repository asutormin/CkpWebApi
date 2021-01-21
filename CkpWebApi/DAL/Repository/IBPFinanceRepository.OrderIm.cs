using System;
using System.Data.Common;

namespace CkpWebApi.DAL.Repository
{
    public partial interface IBPFinanceRepository
    {
        void SetOrderIm(
            DbTransaction dbTran,
            int orderId,
            int orderImTypeId,
            int maketStatusId,
            int maketCategoryId,
            int replaceStatusId,
            string brief,
            bool needVisa,
            bool needVerify,
            int? designerId,
            string comments,
            DateTime? maxClosingDate,
            bool isViewed,
            bool isActual,
            ref DateTime lastEditDate,
            int editUserId);
    }
}
