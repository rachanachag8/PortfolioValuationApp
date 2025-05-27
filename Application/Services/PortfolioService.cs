using System;
using System.Collections.Generic;
using System.Linq;
using PortfolioValuationApp.Core.Constants;
using PortfolioValuationApp.Core.Enums;
using PortfolioValuationApp.Core.Interfaces;
using PortfolioValuationApp.Core.Models;
using PortfolioValuationApp.Infrastructure.Logging;

namespace PortfolioValuationApp.Application.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly IInvestmentRepository _investmentRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly ConsoleLogger _logger;

        public PortfolioService(
            IInvestmentRepository investmentRepo,
            ITransactionRepository transactionRepo,
            IQuoteRepository quoteRepo,
            ConsoleLogger logger)
        {
            _investmentRepository = investmentRepo;
            _transactionRepository = transactionRepo;
            _quoteRepository = quoteRepo;
            _logger = logger;
        }

        public decimal CalculatePortfolioValue(string investorId, DateTime referenceDate)
        {
            var investments = _investmentRepository.GetInvestmentsByInvestor(investorId);
            if (investments == null || !investments.Any())
            {
                return 0;
            }

            decimal totalValue = 0;

            foreach (var investment in investments)
            {
                try
                {
                    totalValue += CalculateInvestmentValue(investment, referenceDate);
                }
                catch (Exception ex)
                {
                    _logger.Error(
                        string.Format(
                            LogMessages.ErrorCalculatingInvestment,
                            investment.InvestmentId
                        ),
                        ex
                    );
                }
            }

            return totalValue;
        }

        private decimal CalculateInvestmentValue(Investment investment, DateTime date)
        {
            return investment.InvestmentType switch
            {
                InvestmentType.Stock => CalculateStockValue(investment, date),
                InvestmentType.Estate or InvestmentType.Building => CalculateRealEstateValue(investment, date),
                InvestmentType.Fonds => CalculateFundValue(investment, date),
                _ => LogUnknownInvestmentType(investment)
            };
        }

        private decimal LogUnknownInvestmentType(Investment investment)
        {
            _logger.Warning(
                string.Format(
                    LogMessages.UnknownInvestmentType,
                    investment.InvestmentId
                )
            );
            return 0;
        }

        private decimal CalculateStockValue(Investment investment, DateTime date)
        {
            var transactions = _transactionRepository
                .GetTransactionsByInvestment(investment.InvestmentId, date)
                .Where(t => t.Type == TransactionType.Shares);

            var totalShares = transactions.Sum(t => t.Value);
            var pricePerShare = _quoteRepository.GetLatestQuotePrice(investment.ISIN, date);

            if (!pricePerShare.HasValue)
            {
                return 0;
            }

            var value = totalShares * pricePerShare.Value;
            return value;
        }

        private decimal CalculateRealEstateValue(Investment investment, DateTime date)
        {
            var transactions = _transactionRepository.GetTransactionsByInvestment(investment.InvestmentId, date);

            decimal estateValue = 0, buildingValue = 0;

            foreach (var tx in transactions)
            {
                if (tx.Type == TransactionType.Estate)
                    estateValue = tx.Value;
                else if (tx.Type == TransactionType.Building)
                    buildingValue = tx.Value;
            }

            var totalValue = estateValue + buildingValue;
            return totalValue;
        }

        private decimal CalculateFundValue(Investment investment, DateTime date)
        {
            var transactions = _transactionRepository
                .GetTransactionsByInvestment(investment.InvestmentId, date)
                .Where(t => t.Type == TransactionType.Percentage);

            decimal totalPercentage = transactions.Sum(t => t.Value);

            var subInvestments = _investmentRepository.GetInvestmentsByFund(investment.InvestmentId);
            if (subInvestments == null || !subInvestments.Any())
            {
                return 0;
            }

            decimal fundTotalValue = 0;

            foreach (var subInvestment in subInvestments)
            {
                fundTotalValue += CalculateInvestmentValue(subInvestment, date);
            }

            var fundValue = fundTotalValue * totalPercentage;
            return fundValue;
        }
    }
}
