
namespace CkpModel.Output
{
    public class AuthInfo
    {
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int ClientLegalPersonId { get; set; }
        public string ClientLegalPersonName { get; set; }
        public bool IsNeedPrepayment { get; set; }
        public string Login { get; set; }
        public string Token { get; set; }
    }
}
