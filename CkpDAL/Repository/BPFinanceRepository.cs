namespace CkpDAL.Repository
{
    public partial class BPFinanceRepository : IBPFinanceRepository
    {
        private readonly BPFinanceContext _context;

        public BPFinanceRepository(BPFinanceContext context)
        {
            _context = context;
        }
    }
}
