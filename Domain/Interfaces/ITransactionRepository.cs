using PortfolioValuationApp.Core.Models;

namespace PortfolioValuationApp.Core.Interfaces
{
    public interface ITransactionRepository
    {
        List<Transaction> GetTransactionsByInvestment(string investmentId, DateTime referenceDate);
    }
}
