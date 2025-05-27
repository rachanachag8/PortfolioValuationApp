using System;

namespace PortfolioValuationApp.Core.Interfaces
{
    public interface IPortfolioService
    {
        decimal CalculatePortfolioValue(string investorId, DateTime date);
    }
}
