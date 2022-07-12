using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using SerenityTask.API.Models.Entities;
using SerenityTask.API.Services.Implementations;
using SerenityTask.UnitTests.Services.Builders;

namespace SerenityTask.UnitTests
{
    public class QuoteTests
    {
        private readonly QuoteService _quoteService;

        private readonly SerenityTaskDbContext _dbContext;

        public QuoteTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<SerenityTaskDbContext>()
                .UseInMemoryDatabase(databaseName: "QuotesData").Options;

            _dbContext = new SerenityTaskDbContext(dbContextOptions);
            _quoteService = Substitute.For<QuoteService>(_dbContext);
        }

        [Test]
        public void GetRandomQuote_ShouldReturnAnyQuote()
        {
            // Arrange
            var firstQuote = new QuoteBuilder().Build();
            var secondQuote = new QuoteBuilder().WithId(2).WithAuthorName("Another AuthorName").Build();
            var thirdQuote = new QuoteBuilder().WithId(3).WithContext("Another Context").Build();

            var quotes = new List<Quote> { firstQuote, secondQuote, thirdQuote };
            _dbContext.Quotes.AddRange(quotes);
            _dbContext.SaveChanges();

            // Act
            var randomQuote = _quoteService.GetRandomQuote();

            // Assert
            Assert.Contains(randomQuote, quotes);
        }
    }
}