using System;
using System.Collections.Generic;
using Moq;
using PortfolioValuationApp.Application.Services;
using PortfolioValuationApp.Core.Enums;
using PortfolioValuationApp.Core.Interfaces;
using PortfolioValuationApp.Core.Models;
using PortfolioValuationApp.Infrastructure.Logging;
using Xunit;

namespace PortfolioValuationApp.Tests
{
    public class PortfolioServiceTests
    {
        private readonly Mock<IInvestmentRepository> _investRepo;
        private readonly Mock<ITransactionRepository> _transRepo;
        private readonly Mock<IQuoteRepository> _quoteRepo;
        private readonly ConsoleLogger _logger;
        private readonly PortfolioService _service;

        public PortfolioServiceTests()
        {
            _investRepo = new Mock<IInvestmentRepository>();
            _transRepo = new Mock<ITransactionRepository>();
            _quoteRepo = new Mock<IQuoteRepository>();
            _logger = new ConsoleLogger(); 

            _service = new PortfolioService(
                _investRepo.Object,
                _transRepo.Object,
                _quoteRepo.Object,
                _logger
            );
        }

        [Fact]
        public void CalculatePortfolioValue_NoInvestments_ReturnsZero()
        {
            // Arrange
            _investRepo.Setup(r => r.GetInvestmentsByInvestor("INV1"))
                       .Returns(new List<Investment>());

            // Act
            var result = _service.CalculatePortfolioValue("INV1", DateTime.Today);

            // Assert
            Assert.Equal(0m, result);
        }

        [Fact]
        public void CalculatePortfolioValue_SingleStock_ReturnsSharesTimesPrice()
        {
            // Arrange
            var inv = new Investment
            {
                InvestmentId = "I1",
                InvestorId = "INV1",
                InvestmentType = InvestmentType.Stock,
                ISIN = "ISIN1"
            };
            _investRepo.Setup(r => r.GetInvestmentsByInvestor("INV1"))
                       .Returns(new List<Investment> { inv });
            _transRepo.Setup(r => r.GetTransactionsByInvestment("I1", It.IsAny<DateTime>()))
                      .Returns(new List<Transaction>
                      {
                          new Transaction { InvestmentId = "I1", Type = TransactionType.Shares, Date = DateTime.Today, Value = 5 }
                      });
            _quoteRepo.Setup(r => r.GetLatestQuotePrice("ISIN1", It.IsAny<DateTime>()))
                      .Returns(10m);

            // Act
            var result = _service.CalculatePortfolioValue("INV1", DateTime.Today);

            // Assert
            Assert.Equal(5 * 10m, result);
        }

        [Fact]
        public void CalculatePortfolioValue_RealEstate_ReturnsEstatePlusBuilding()
        {
            // Arrange
            var inv = new Investment
            {
                InvestmentId = "E1",
                InvestorId = "INV1",
                InvestmentType = InvestmentType.Estate
            };
            _investRepo.Setup(r => r.GetInvestmentsByInvestor("INV1"))
                       .Returns(new List<Investment> { inv });
            _transRepo.Setup(r => r.GetTransactionsByInvestment("E1", It.IsAny<DateTime>()))
                      .Returns(new List<Transaction>
                      {
                          new Transaction { InvestmentId = "E1", Type = TransactionType.Estate, Date = DateTime.Today, Value = 100 },
                          new Transaction { InvestmentId = "E1", Type = TransactionType.Building, Date = DateTime.Today, Value = 50 }
                      });

            // Act
            var result = _service.CalculatePortfolioValue("INV1", DateTime.Today);

            // Assert
            Assert.Equal(150m, result);
        }

        [Fact]
        public void CalculatePortfolioValue_FundWithSubInvestments_CalculatesCorrectly()
        {
            // Arrange
            var fund = new Investment
            {
                InvestmentId = "I1",
                InvestorId = "INV1",
                InvestmentType = InvestmentType.Fonds
            };
            var sub1 = new Investment
            {
                InvestmentId = "I2",
                InvestorId = "INV1",
                InvestmentType = InvestmentType.Stock,
                ISIN = "S1"
            };
            var sub2 = new Investment
            {
                InvestmentId = "I3",
                InvestorId = "INV1",
                InvestmentType = InvestmentType.Stock,
                ISIN = "S2"
            };

            _investRepo.Setup(r => r.GetInvestmentsByInvestor("INV1"))
                       .Returns(new List<Investment> { fund });
            _transRepo.Setup(r => r.GetTransactionsByInvestment("I1", It.IsAny<DateTime>()))
                      .Returns(new List<Transaction>
                      {
                          new Transaction { InvestmentId = "I1", Type = TransactionType.Percentage, Date = DateTime.Today, Value = 0.5m }
                      });
            _investRepo.Setup(r => r.GetInvestmentsByFund("I1"))
                       .Returns(new List<Investment> { sub1, sub2 });

            _transRepo.Setup(r => r.GetTransactionsByInvestment("I2", It.IsAny<DateTime>()))
                      .Returns(new List<Transaction> { new Transaction { InvestmentId = "I2", Type = TransactionType.Shares, Date = DateTime.Today, Value = 2 } });
            _quoteRepo.Setup(r => r.GetLatestQuotePrice("S1", It.IsAny<DateTime>()))
                      .Returns(10m);

            _transRepo.Setup(r => r.GetTransactionsByInvestment("I3", It.IsAny<DateTime>()))
                      .Returns(new List<Transaction> { new Transaction { InvestmentId = "I3", Type = TransactionType.Shares, Date = DateTime.Today, Value = 3 } });
            _quoteRepo.Setup(r => r.GetLatestQuotePrice("S2", It.IsAny<DateTime>()))
                      .Returns(20m);

            // Act
            var result = _service.CalculatePortfolioValue("INV1", DateTime.Today);

           
            Assert.Equal(40m, result);
        }

        [Fact]
        public void CalculatePortfolioValue_UnknownType_ReturnsZero()
        {
            // Arrange
            var inv = new Investment
            {
                InvestmentId = "U1",
                InvestorId = "INV1",
                InvestmentType = (InvestmentType)999  
            };
            _investRepo.Setup(r => r.GetInvestmentsByInvestor("INV1"))
                       .Returns(new List<Investment> { inv });

            // Act
            var result = _service.CalculatePortfolioValue("INV1", DateTime.Today);

            // Assert
            Assert.Equal(0m, result);
        }
    }
}
