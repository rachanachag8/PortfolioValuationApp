using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using PortfolioValuationApp.Core.Enums;
using PortfolioValuationApp.Core.Models;
using PortfolioValuationApp.Infrastructure.Logging;
using PortfolioValuationApp.Infrastructure.Repositories;
using Xunit;

namespace PortfolioValuationApp.Tests
{
    public class TransactionRepositoryTests : IDisposable
    {
        private readonly string _csvPath;
        private readonly ConsoleLogger _logger;
        private readonly TransactionRepository _repository;

        public TransactionRepositoryTests()
        {
            // Create a temporary CSV file
            _csvPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");
            File.WriteAllLines(_csvPath, new[]
            {
                "InvestmentId;Type;Date;Value",
                "I1;Shares;2021-01-01;5",
                "I1;Shares;2021-01-05;3",
                "I1;Estate;2021-01-02;100",
                "I2;Shares;2021-01-01;10"
            });

            _logger = new ConsoleLogger();
            _repository = new TransactionRepository(_csvPath, _logger);
        }

        [Fact]
        public void GetTransactionsByInvestment_ExactDate_IncludesThatDate()
        {
            // Act
            var list = _repository.GetTransactionsByInvestment("I1",
                DateTime.ParseExact("2021-01-05", "yyyy-MM-dd", CultureInfo.InvariantCulture));

            // Assert
            Assert.Equal(3, list.Count);
            Assert.Contains(list, t => t.Type == TransactionType.Shares && t.Value == 3);
            Assert.Contains(list, t => t.Type == TransactionType.Estate && t.Value == 100);
        }

        [Fact]
        public void GetTransactionsByInvestment_BeforeSomeDates_FiltersCorrectly()
        {
            // Act: reference date 2021-01-02 should exclude the 1/5 shares
            var list = _repository.GetTransactionsByInvestment("I1",
                DateTime.ParseExact("2021-01-02", "yyyy-MM-dd", CultureInfo.InvariantCulture));

            // Assert
            Assert.Equal(2, list.Count);
            Assert.DoesNotContain(list, t => t.Date == DateTime.Parse("2021-01-05", CultureInfo.InvariantCulture));
        }

        [Fact]
        public void GetTransactionsByInvestment_UnknownInvestment_ReturnsEmpty()
        {
            // Act
            var list = _repository.GetTransactionsByInvestment("UNKNOWN", DateTime.Today);

            // Assert
            Assert.Empty(list);
        }

        [Fact]
        public void GetTransactionsByInvestment_DifferentInvestmentId_IgnoresOthers()
        {
            // Act
            var list = _repository.GetTransactionsByInvestment("I2",
                DateTime.ParseExact("2021-01-10", "yyyy-MM-dd", CultureInfo.InvariantCulture));

            // Assert
            Assert.Single(list);
            Assert.Equal("I2", list[0].InvestmentId);
            Assert.Equal(10, list[0].Value);
        }

        public void Dispose()
        {
            // Clean up temporary file
            if (File.Exists(_csvPath))
                File.Delete(_csvPath);
        }
    }
}
