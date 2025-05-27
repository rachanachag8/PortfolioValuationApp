using System;
using System.Globalization;
using Domain.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PortfolioValuationApp.Application.Services;
using PortfolioValuationApp.Core.Constants;
using PortfolioValuationApp.Core.Interfaces;
using PortfolioValuationApp.Infrastructure.Logging;
using PortfolioValuationApp.Infrastructure.Repositories;

namespace PortfolioValuationApp.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            const string investmentsPath = Constantvalues.InvestmentsCsvFilePath;
            const string transactionsPath = Constantvalues.TransactionsCsvFilePath;
            const string quotesPath = Constantvalues.QuotesCsvFilePath;

            var services = new ServiceCollection();

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            services.AddSingleton<ConsoleLogger>();

            // Add Repositories with file paths and logger
            services.AddSingleton<IInvestmentRepository>(provider =>
                new InvestmentRepository(investmentsPath, provider.GetRequiredService<ConsoleLogger>()));

            services.AddSingleton<ITransactionRepository>(provider =>
                new TransactionRepository(transactionsPath, provider.GetRequiredService<ConsoleLogger>()));

            services.AddSingleton<IQuoteRepository>(provider =>
                new QuoteRepository(quotesPath, provider.GetRequiredService<ConsoleLogger>()));

            // Add Application Service
            services.AddSingleton<IPortfolioService, PortfolioService>();

            // Build service provider
            var serviceProvider = services.BuildServiceProvider();

            // Get logger and service
            var logger = serviceProvider.GetService<ILogger<Program>>();
            var portfolioService = serviceProvider.GetService<IPortfolioService>();


            try
            {
                Console.Write("Enter Investor ID: ");
                string investorId = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(investorId))
                {
                    logger.LogError(LogMessages.InvestorIdEmpty);
                    Console.WriteLine(LogMessages.InvestorIdEmpty);
                    return;
                }

                Console.Write("Enter Reference Date (yyyy-MM-dd): ");
                string dateInput = Console.ReadLine();

                if (!DateTime.TryParseExact(dateInput, LogMessages.dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var referenceDate))
                {
                    logger.LogError(LogMessages.InvalidDateFormat);
                    Console.WriteLine(LogMessages.InvalidDateFormat);
                    return;
                }

                logger.LogInformation(string.Format(LogMessages.PortfolioValueCalculation, investorId, referenceDate));

                var portfolioValue = portfolioService.CalculatePortfolioValue(investorId, referenceDate);


                Console.WriteLine($"Portfolio value for Investor {investorId} on {referenceDate:yyyy-MM-dd}: {Constantvalues.DefaultCurrency}{portfolioValue:N2}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, LogMessages.PortfolioCalculationError);
                Console.WriteLine(LogMessages.PortfolioCalculationError);
            }

        }
    }
}
