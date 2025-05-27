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
    public class TransactionRepository : ITransactionRepository
    {
        private readonly string _filePath;
        private readonly ConsoleLogger _logger;
        private List<Transaction> _transactions;

        public TransactionRepository(string filePath, ConsoleLogger logger)
        {
            _filePath = filePath;
            _logger = logger;
            _transactions = new List<Transaction>(); 
        }

        private void LoadTransactionsFromCsv()
        {
            if (_transactions.Count == 0) 
            {
                try
                {
                    _logger.Info(string.Format(LogMessages.ReadingInvestments, _filePath));
                    _transactions = CsvReader.ReadCsv<Transaction>(_filePath);
                    _logger.Info(string.Format(LogMessages.InvestmentRecordsLoaded, _transactions.Count));
                }
                catch (Exception ex)
                {
                    _logger.Error(LogMessages.FailedToLoad, ex);
                    _transactions = new List<Transaction>(); 
                }
            }
        }

        public List<Transaction> GetTransactionsByInvestment(string investmentId, DateTime referenceDate)
        {
            LoadTransactionsFromCsv(); 
            return _transactions
                .Where(t => t.InvestmentId == investmentId && t.Date <= referenceDate)
                .ToList();
        }
    }
}
