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

        public float GetDiscount(int legalPersonId, int pricePositionTypeId)
        {
            var pricePositionTypeDiscount = _context.LegalPersons
                .Include(lp => lp.PricePositionTypeDiscounts)
                .Single(lp => lp.Id == legalPersonId)
                .PricePositionTypeDiscounts
                .SingleOrDefault(pptd => pptd.PricePositionTypeId == pricePositionTypeId);

            return pricePositionTypeDiscount == null
                ? 0
                : pricePositionTypeDiscount.DiscountPercent;
        }
    }
}
