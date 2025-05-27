using System.Collections.Generic;
using PortfolioValuationApp.Core.Models;

namespace PortfolioValuationApp.Core.Interfaces
{
    public interface IInvestmentRepository
    {
        List<Investment> GetAll();
        Investment GetById(string investmentId);
        List<Investment> GetInvestmentsByInvestor(string investorId);
        List<Investment> GetInvestmentsByFund(string fundInvestor);  
    }
}
