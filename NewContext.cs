using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication3.Models;
using Bogus;
namespace WebApplication3
{

    public class NewContext: DbContext
    {
        public NewContext(DbContextOptions<NewContext> options) : base(options)
        {
        }
        public DbSet<New> NewModels { get; set; }
        public Faker<New> NewFaker { get; set; } = new Faker<New>()
            
            .RuleFor(x => x.title, f => f.Lorem.Sentence(3))
            .RuleFor(x => x.summary, f => f.Lorem.Sentence(10))
            .RuleFor(x => x.content, f => f.Lorem.Paragraph(1))
            .RuleFor(x => x.slug, f => f.Lorem.Slug(5))
            .RuleFor(x => x.image, f => f.Image.PicsumUrl());
    }
    
    
}
