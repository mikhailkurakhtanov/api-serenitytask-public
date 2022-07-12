using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SerenityTask.API.Models.Entities;

namespace SerenityTask.API.DataSeed
{
    public class QuotesConfiguration : IEntityTypeConfiguration<Quote>
    {
        public void Configure(EntityTypeBuilder<Quote> builder)
        {
            builder.ToTable("Quotes");

            builder.HasData
            (
                new Quote
                {
                    Id = 1,
                    AuthorName = "Harriet Beecher Stowe",
                    Context = "“Never give up, for that is just the place and time that the tide will turn.”"
                },
                new Quote
                {
                    Id = 2,
                    AuthorName = "Elbert Hubbard",
                    Context = "“There is no failure except in no longer trying.”"
                },
                new Quote
                {
                    Id = 3,
                    AuthorName = "James A. Michener",
                    Context = "“Character consists of what you do on the third and fourth tries.”"
                },
                new Quote
                {
                    Id = 4,
                    AuthorName = "Chuck Yeager",
                    Context = "“You do what you can for as long as you can, and when you finally can’t, " +
                    "you do the next best thing. You back up but you don’t give up.”"
                },
                new Quote
                {
                    Id = 5,
                    AuthorName = "Roy T. Bennett",
                    Context = "“Do not fear failure but rather fear not trying.”"
                }
            );
        }
    }
}
