using Domain.EF.Entities.Main;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.EF.Configuration
{
    public class ThreadConfig : IEntityTypeConfiguration<Thread>
    {
        public void Configure(EntityTypeBuilder<Thread> builder)
            => builder.HasOne(e => e.ParentThread)
            .WithMany(e => e.ChildThreads)
            .HasForeignKey(e => e.ParentId)
            .HasPrincipalKey(e => e.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
