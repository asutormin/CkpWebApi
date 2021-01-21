
using System.Collections.Generic;

namespace CkpWebApi.Helpers
{
    public class AppParams
    {
        public int OrderBusinessUnitId { get; set; }
        public int PriceBusinessUnitId { get; set; }
        public int PricePermissionFlag { get; set; }
        public int EditUserId { get; set; }
        public int ManagerId { get; set; }
        public string ShoppingCartOrderDescription { get; set; }
        public List<int> Suppliers { get; set; }
    }
}
