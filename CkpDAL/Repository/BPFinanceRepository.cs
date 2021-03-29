namespace CkpDAL.Repository
{
    public partial class BPFinanceRepository : IBPFinanceRepository
    {
        private readonly BPFinanceContext _context;
        private readonly int _editUserId;

        public BPFinanceRepository(BPFinanceContext context, int editUserId)
        {
            _context = context;
            _editUserId = editUserId;
        }
    }
}
