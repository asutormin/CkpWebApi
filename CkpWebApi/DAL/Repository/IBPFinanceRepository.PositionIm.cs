using System;
using System.Data.Common;

namespace CkpWebApi.DAL.Repository
{
    public partial interface IBPFinanceRepository
    {
        void SetPositionIm(
            DbTransaction dbTran,
            int orderId,
            int orderPositionId,
            int positionImTypeId,
            int? parentPositionId,
            int maketStatusId,
            int maketCategoryId,
            string text,
            string comments,
            string url,
            string distributionUrl,
            int? legalPersonPersonalDataId,
            string xml,
            int rating,
            string ratingDescription,
            bool newTaskFile,
            bool newMaketFile,
            ref DateTime? taskFileDate,
            ref DateTime? maketFileDate,
            bool dontVerify,
            bool rdvRating,
            bool isActual,
            ref DateTime lastEditDate,
            int editUserId);
    }
}
