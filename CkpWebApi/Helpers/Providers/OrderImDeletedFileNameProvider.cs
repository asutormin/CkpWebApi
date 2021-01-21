using CkpWebApi.Infrastructure.Providers.Interfaces;
using System;

namespace CkpWebApi.Helpers.Providers
{
    public class OrderImDeletedFileNameProvider : IKeyedProvider<DateTime, string>
    {
        private readonly string _template;

        public OrderImDeletedFileNameProvider(string template)
        {
            _template = template;
        }
        public string GetByValue(DateTime deleteDate)
        {
            var fileName = string.Format("{0}_deleted_{1}_{2}", 
                _template, 
                deleteDate.ToString("yyyyMMdd"), 
                deleteDate.ToString("HHmmss"));

            return fileName;
        }
    }
}
