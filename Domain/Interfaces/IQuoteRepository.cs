using System;
using PortfolioValuationApp.Core.Models;

namespace PortfolioValuationApp.Core.Interfaces
{
    public interface IQuoteRepository
    {
        decimal? GetLatestQuotePrice(string isin, DateTime date);
    }
}
