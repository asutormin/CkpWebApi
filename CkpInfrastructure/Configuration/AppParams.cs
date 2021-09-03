using System.Collections.Generic;

namespace CkpInfrastructure.Configuration
{
    public class AppParams
    {
        public List<OrderSettings> OrderSettings { get; set; }
        public int PricePermissionFlag { get; set; }
        public int EditUserId { get; set; }
        public int ManagerId { get; set; }
        public string BasketOrderDescription { get; set; }        
    }
}
