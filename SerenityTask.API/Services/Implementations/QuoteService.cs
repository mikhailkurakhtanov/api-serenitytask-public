using System;
using System.Linq;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.Services.Implementations
{
    public class QuoteService : IQuoteService
    {
        private readonly SerenityTaskDbContext _dbContext;

        public QuoteService(SerenityTaskDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Quote GetRandomQuote()
        {
            var random = new Random();
            var quotes = _dbContext.Quotes.ToList();
            if (!quotes.Any()) return null;

            var randomQuotePosition = random.Next(quotes.Count);
            var randomQuote = quotes[randomQuotePosition];

            return randomQuote;
        }
    }
}
