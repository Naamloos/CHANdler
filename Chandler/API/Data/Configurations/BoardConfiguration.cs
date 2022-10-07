using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations
{
    public class BoardConfiguration : BaseEntityConfiguration<Board>
    {
        public override void Configure(EntityTypeBuilder<Board> builder)
        {
            builder.Property(e => e.Tag)
                .HasColumnType("NVARCHAR(45)")
                .IsRequired();

            builder.Property(e => e.Title)
                .HasColumnType("NVARCHAR(45)")
                .IsRequired();

            builder.Property(e => e.Description)
                .HasColumnType("NVARCHAR(250)");

            builder.Property(e => e.BannerBase64)
                .HasColumnType("VARCHAR(MAX)");

            builder.Property(e => e.RequireImages)
                .HasColumnType("BIT")
                .HasDefaultValue(false);

            builder.Property(e => e.IsNSFW)
                .HasColumnType("BIT")
                .HasDefaultValue(false);

            base.Configure(builder);
        }
    }
}
