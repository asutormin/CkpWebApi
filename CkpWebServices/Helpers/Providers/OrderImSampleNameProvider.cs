using CkpInfrastructure.Providers.Interfaces;
using System;

namespace CkpServices.Helpers.Providers
{
    public class OrderImSampleNameProvider : IKeyedProvider<Tuple<int, DateTime, string>, string>
    {
        private readonly string _template;
        
        public OrderImSampleNameProvider(string template)
        {
            _template = template;
        }

        public string GetByValue(Tuple<int, DateTime, string> value)
        {
            var orderPositionIdString = value.Item1.ToString();
            var dateString = value.Item2.ToString("yyyyMMdd");
            var timeString = value.Item2.TimeOfDay.ToString("hhmmss");
            var name = value.Item3;

            return string.Format(_template, orderPositionIdString.Substring(0, 3), orderPositionIdString, dateString, timeString, name);
        }
    }
}
