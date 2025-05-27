using System;
using System.Collections.Generic;
using System.Linq;
using PortfolioValuationApp.Core.Constants;
using PortfolioValuationApp.Core.Interfaces;
using PortfolioValuationApp.Core.Models;
using PortfolioValuationApp.Core.Utilities;
using PortfolioValuationApp.Infrastructure.Logging;

namespace PortfolioValuationApp.Infrastructure.Repositories
{
    public class InvestmentRepository : IInvestmentRepository
    {
        private readonly string _filePath;
        private readonly ConsoleLogger _logger;
        private List<Investment> _investments;

        public InvestmentRepository(string filePath, ConsoleLogger logger)
        {
            _filePath = filePath;
            _logger = logger;
            _investments = new List<Investment>();
        }

        private void LoadInvestmentsFromCsv()
        {
            if (_investments.Count == 0)
            {
                try
                {
                    _logger.Info(string.Format(LogMessages.ReadingInvestments, _filePath));
                    _investments = CsvReader.ReadCsv<Investment>(_filePath);
                    _logger.Info(string.Format(LogMessages.InvestmentRecordsLoaded, _investments.Count));
                }
                catch (Exception ex)
                {
                    _logger.Error(LogMessages.FailedToLoad, ex);
                    _investments = new List<Investment>();
                }
            }
        }

        public List<Investment> GetAll()
        {
            LoadInvestmentsFromCsv();
            return _investments;
        }

        public Investment GetById(string investmentId)
        {
            LoadInvestmentsFromCsv();
            return _investments.FirstOrDefault(i =>
                string.Equals(i.InvestmentId, investmentId, StringComparison.OrdinalIgnoreCase));
        }

        public List<Investment> GetInvestmentsByInvestor(string investorId)
        {
            LoadInvestmentsFromCsv();
            return _investments.Where(i =>
                string.Equals(i.InvestorId, investorId, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<Investment> GetInvestmentsByFund(string fundInvestor)
        {
            LoadInvestmentsFromCsv();
            return _investments.Where(i =>
                string.Equals(i.FundInvestor, fundInvestor, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
