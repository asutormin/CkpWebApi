using CkpDAL;
using CkpInfrastructure.Providers.Interfaces;
using System;

namespace CkpServices.Helpers.Providers
{
    public class BasketBusinessUnitIdProvider : IKeyedProvider<Tuple<int, int>, int>
    {
        private readonly IKeyedProvider<int, int> _interactionBusinessUnitIdProvider;
        private readonly IKeyedProvider<int, int> _businessUnitByPriceIdProvider;

        public BasketBusinessUnitIdProvider(BPFinanceContext context)
        {
            _interactionBusinessUnitIdProvider = new InteractionBusinessUnitIdProvider(context);
            _businessUnitByPriceIdProvider = new BusinessUnitIdByPriceIdProvider(context);
        }

        public int GetByValue(Tuple<int, int> value)
        {
            var clientLegalPersonId = value.Item1;
            var priceId = value.Item2;

            // Ищем бизнес юнит заказ в настройках счёта юр. лица клиента
            var businessUnitId = _interactionBusinessUnitIdProvider.GetByValue(clientLegalPersonId);

            // Если бизнес юнит не задан - используем бизнес юнит цены
            if (businessUnitId == 0)
                businessUnitId = _businessUnitByPriceIdProvider.GetByValue(priceId);

            return businessUnitId;
        }
    }
}
