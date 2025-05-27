using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using PortfolioValuationApp.Core.Models;
using PortfolioValuationApp.Infrastructure.Logging;
using PortfolioValuationApp.Infrastructure.Repositories;
using Xunit;

namespace PortfolioValuationApp.Tests
{
    public class QuoteRepositoryTests : IDisposable
    {
        private readonly string _csvPath;
        private readonly ConsoleLogger _logger;
        private readonly QuoteRepository _repository;

        public QuoteRepositoryTests()
        {
            // Create a temporary CSV file
            _csvPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");
            File.WriteAllLines(_csvPath, new[]
            {
                "ISIN;Date;PricePerShare",
                "ISIN1;2021-01-01;100.5",
                "ISIN1;2021-01-02;101.0",
                "ISIN1;2020-12-31;99.0",
                "ISIN2;2021-01-01;200.0"
            });

            _logger = new ConsoleLogger();
            _repository = new QuoteRepository(_csvPath, _logger);
        }

        [Fact]
        public void GetLatestQuotePrice_ExactDateMatch_ReturnsCorrectPrice()
        {
            // Act
            var price = _repository.GetLatestQuotePrice("ISIN1", DateTime.ParseExact("2021-01-02", "yyyy-MM-dd", CultureInfo.InvariantCulture));

            // Assert
            Assert.Equal(101.0m, price);
        }

        [Fact]
        public void GetLatestQuotePrice_EarlierDate_ReturnsMostRecentBeforeDate()
        {
            // Act: request 2021-01-03, should return 101.0
            var price = _repository.GetLatestQuotePrice("ISIN1", DateTime.ParseExact("2021-01-03", "yyyy-MM-dd", CultureInfo.InvariantCulture));

            // Assert
            Assert.Equal(101.0m, price);
        }

        [Fact]
        public void GetLatestQuotePrice_BeforeAnyDate_ReturnsNull()
        {
            // Act: request before earliest date
            var price = _repository.GetLatestQuotePrice("ISIN1", DateTime.ParseExact("2020-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture));

            // Assert
            Assert.Null(price);
        }

        [Fact]
        public void GetLatestQuotePrice_UnknownIsin_ReturnsNull()
        {
            // Act
            var price = _repository.GetLatestQuotePrice("UNKNOWN", DateTime.Today);

            // Assert
            Assert.Null(price);
        }

        public void Dispose()
        {
            // Clean up temporary file
            if (File.Exists(_csvPath))
                File.Delete(_csvPath);
        }
    }
}
