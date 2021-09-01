using CkpDAL;
using CkpServices.Processors.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CkpServices.Processors
{
    class ClientProcessor : IClientProcessor
    {
        private readonly BPFinanceContext _context;

        public ClientProcessor(BPFinanceContext context)
        {
            _context = context;
        }

        public float GetDiscount(int legalPersonId, int businessUnitId, int supplierId, int pricePositionTypeId)
        {
            var pricePositionTypeDiscount = _context.LegalPersons
                .Include(lp => lp.PersonalDiscounts)
                .Single(lp => lp.Id == legalPersonId)
                .PersonalDiscounts
                .SingleOrDefault(
                    pd =>
                        pd.BusinessUnitId == businessUnitId &&
                        pd.SupplierId == supplierId &&
                        pd.PricePositionTypeId == pricePositionTypeId);

            return pricePositionTypeDiscount == null
                ? 0
                : pricePositionTypeDiscount.DiscountPercent;
        }
    }
}
