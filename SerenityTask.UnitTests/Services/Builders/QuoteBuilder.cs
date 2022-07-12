using SerenityTask.API.Models.Entities;

namespace SerenityTask.UnitTests.Services.Builders
{
    public class QuoteBuilder
    {
        private long _id;

        private string _authorName;

        private string _context;

        public QuoteBuilder()
        {
            _id = 1;
            _context = "Default Context";
            _authorName = "Default Author";
        }

        public Quote Build()
        {
            return new Quote
            {
                Id = _id,
                AuthorName = _authorName,
                Context = _context
            };
        }

        public QuoteBuilder WithAuthorName(string authorName)
        {
            _authorName = authorName;
            return this;
        }

        public QuoteBuilder WithId(long id)
        {
            _id = id;
            return this;
        }

        public QuoteBuilder WithContext(string context)
        {
            _context = context;
            return this;
        }
    }
}
