using PortfolioValuationApp.Core.Enums;

namespace PortfolioValuationApp.Core.Models
{
    public class Transaction
    {
        public string InvestmentId { get; set; }
        public TransactionType Type { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
    }
}
