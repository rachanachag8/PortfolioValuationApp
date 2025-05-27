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
    public class QuoteRepository : IQuoteRepository
    {
        private readonly string _filePath;
        private readonly ConsoleLogger _logger;
        private List<Quote> _quotes;

        public QuoteRepository(string filePath, ConsoleLogger logger)
        {
            _filePath = filePath;
            _logger = logger;
            _quotes = new List<Quote>();
        }

        private void LoadQuotesFromCsv()
        {
            if (_quotes.Count == 0)
            {
                try
                {
                    _logger.Info(string.Format(LogMessages.ReadingInvestments, _filePath));
                    _quotes = CsvReader.ReadCsv<Quote>(_filePath);
                    _logger.Info(string.Format(LogMessages.InvestmentRecordsLoaded, _quotes.Count));
                }
                catch (Exception ex)
                {
                    _logger.Error(LogMessages.FailedToLoad, ex);
                    _quotes = new List<Quote>(); 
                }
            }
        }

        public decimal? GetLatestQuotePrice(string isin, DateTime date)
        {
            LoadQuotesFromCsv();

            var quotesForIsin = _quotes
                .Where(q => q.ISIN == isin && q.Date <= date)
                .OrderByDescending(q => q.Date)
                .ToList();

            if (!quotesForIsin.Any())
            {
                return null;
            }

            return quotesForIsin.First().PricePerShare;
        }
    }
}
