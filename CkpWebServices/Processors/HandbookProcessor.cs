using CkpDAL;
using CkpDAL.Entities.String;
using CkpDAL.Repository;
using CkpServices.Processors.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace CkpServices.Processors
{
    public class HandbookProcessor : IHandbookProcessor
    {
        private readonly BPFinanceContext _context;
        private readonly IBPFinanceRepository _repository;

        public HandbookProcessor(BPFinanceContext context, IBPFinanceRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        #region Education

        public string GetEducation(int educationId)
        {
            var eduction = GetHandbookItemValue(1, educationId);

            return eduction;
        }

        public List<Handbook> GetEducations()
        {
            return GetHandbook(1);
        }

        #endregion

        #region Experience

        public string GetExperience(int experienceId)
        {
            var experience = GetHandbookItemValue(2, experienceId);

            return experience;
        }

        public List<Handbook> GetExperiences()
        {
            return GetHandbook(2);
        }

        #endregion

        #region WorkGraphic

        public string GetWorkGraphic(int workGraphicId)
        {
            var workGraphic = GetHandbookItemValue(4, workGraphicId);

            return workGraphic;
        }

        public List<Handbook> GetWorkGraphics()
        {
            return GetHandbook(4);
        }

        #endregion

        #region Currency

        public string GetCurrency(int currencyId)
        {
            var currency = GetHandbookItemValue(6, currencyId);

            return currency;
        }

        public List<Handbook> GetCurrencies()
        {
            return GetHandbook(6);
        }

        #endregion

        private List<Handbook> GetHandbook(int handbookTypeId)
        {
            return _context.Handbooks
             .Where(h => h.HandbookTypeId == handbookTypeId)
             .ToList();
        }

        private string GetHandbookItemValue(int handbookTypeId, int handbookId)
        {
            var value = _context.Handbooks
                .SingleOrDefault(
                    h =>
                        h.HandbookTypeId == handbookTypeId && h.Id == handbookId)
                .HandbookValue;

            return value;
        }
    }
}
