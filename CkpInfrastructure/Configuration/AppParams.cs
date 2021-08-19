using System.Collections.Generic;

namespace CkpInfrastructure.Configuration
{
    public class AppParams
    {
        public int OrderBusinessUnitId { get; set; }
        public int PriceBusinessUnitId { get; set; }
        public int PricePermissionFlag { get; set; }
        public int EditUserId { get; set; }
        public int ManagerId { get; set; }
        public string BasketOrderDescription { get; set; }
        public List<int> Suppliers { get; set; }
    }
}
