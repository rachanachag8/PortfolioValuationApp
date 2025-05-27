using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PortfolioValuationApp.Core.Enums;
using PortfolioValuationApp.Core.Models;
using PortfolioValuationApp.Infrastructure.Logging;
using PortfolioValuationApp.Infrastructure.Repositories;
using Xunit;

namespace PortfolioValuationApp.Tests
{
    public class InvestmentRepositoryTests : IDisposable
    {
        private readonly string _csvPath;
        private readonly ConsoleLogger _logger;
        private readonly InvestmentRepository _repository;

        public InvestmentRepositoryTests()
        {
            // Create a temporary CSV file
            _csvPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.csv");
            File.WriteAllLines(_csvPath, new[]
            {
                "InvestorId;InvestmentId;InvestmentType;ISIN;City;FundInvestor",
                "INV1;I1;Stock;S1;;F1",
                "INV1;I2;Estate;;CityA;F1",
                "INV2;I3;Fonds;;;F2"
            });

            _logger = new ConsoleLogger();
            _repository = new InvestmentRepository(_csvPath, _logger);
        }

        [Fact]
        public void GetAll_ShouldReturnAllRecords()
        {
            // Act
            var all = _repository.GetAll();

            // Assert
            Assert.Equal(3, all.Count);
            Assert.Contains(all, i => i.InvestmentId == "I1");
            Assert.Contains(all, i => i.InvestmentId == "I2");
            Assert.Contains(all, i => i.InvestmentId == "I3");
        }

        [Fact]
        public void GetById_KnownId_ShouldReturnInvestment()
        {
            // Act
            var inv = _repository.GetById("I2");

            // Assert
            Assert.NotNull(inv);
            Assert.Equal("INV1", inv.InvestorId);
            Assert.Equal(InvestmentType.Estate, inv.InvestmentType);
            Assert.Equal("CityA", inv.City);
        }

        [Fact]
        public void GetById_UnknownId_ShouldReturnNull()
        {
            // Act
            var inv = _repository.GetById("X");

            // Assert
            Assert.Null(inv);
        }

        [Fact]
        public void GetInvestmentsByInvestor_ShouldFilterCorrectly()
        {
            // Act
            var inv1 = _repository.GetInvestmentsByInvestor("INV1");
            var inv2 = _repository.GetInvestmentsByInvestor("INV2");

            // Assert
            Assert.Equal(2, inv1.Count);
            Assert.Single(inv2);
            Assert.Equal("I3", inv2[0].InvestmentId);
        }

        [Fact]
        public void GetInvestmentsByFund_ShouldFilterCorrectly()
        {
            // Act
            var f1 = _repository.GetInvestmentsByFund("F1");
            var f2 = _repository.GetInvestmentsByFund("F2");

            // Assert
            Assert.Equal(2, f1.Count);
            Assert.Single(f2);
            Assert.Equal("I3", f2[0].InvestmentId);
        }

        public void Dispose()
        {
            // Clean up temporary file
            if (File.Exists(_csvPath))
                File.Delete(_csvPath);
        }
    }
}
