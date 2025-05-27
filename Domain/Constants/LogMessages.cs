namespace PortfolioValuationApp.Core.Constants
{
    public static class LogMessages
    {
        public const string ReadingInvestments = "Reading investments from: {0}";
        public const string InvestmentRecordsLoaded = "{0} investment records loaded.";
        public const string FailedToLoad = "Failed to load data.";
        public const string ErrorCalculatingInvestment = "Error calculating value for investment {0}";
        public const string UnknownInvestmentType = "Unknown investment type for ID: {0}";
        public const string CsvFileNotFound = "CSV file not found: {0}";
        public const string CsvFileEmptyOrCorrupt = "CSV file is empty or corrupt.";
        public const string ErrorReadingCsv = "Error reading CSV: {0}";
        //program file
        public const string InvestorIdEmpty = "Investor ID cannot be empty.";
        public const string InvalidDateFormat = "Invalid date format. Please use yyyy-MM-dd.";
        public const string PortfolioValueCalculation = "Calculating portfolio value for Investor {0} on {1:yyyy-MM-dd}";
        public const string PortfolioValueSuccess = "Portfolio value for Investor {0} on {1:yyyy-MM-dd}: {2:C}";
        public const string PortfolioCalculationError = "An error occurred while calculating the portfolio value.";

        //dateformat
        public const string dateFormat = "yyyy-MM-dd";

    }

}
