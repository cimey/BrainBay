using BrainBay.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrainBay.Infrastructure.Configurations
{
    public class CharacterEntityConfiguration : IEntityTypeConfiguration<Character>
    {
        public void Configure(EntityTypeBuilder<Character> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            // Configure owned type: Origin
            builder.OwnsOne(c => c.Origin);

            // Configure owned type: Location
            builder.OwnsOne(c => c.Location);

            builder.Property(c => c.Status).HasMaxLength(50);
            builder.Property(c => c.Species).HasMaxLength(50);
            builder.Property(c => c.Type).HasMaxLength(50);
            builder.Property(c => c.Gender).HasMaxLength(20);
            builder.Property(c => c.Image).HasMaxLength(300);
            builder.Property(c => c.Episodes);
            builder.Property(c => c.CreatedAt);
        }
    }
}
