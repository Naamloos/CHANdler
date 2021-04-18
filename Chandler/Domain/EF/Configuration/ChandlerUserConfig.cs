using Domain.EF.Entities.Main;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.EF.Configuration
{
    public class ChandlerUserConfig : IEntityTypeConfiguration<ChandlerUser>
    {
        public void Configure(EntityTypeBuilder<ChandlerUser> builder)
        {
            builder.HasOne(e => e.AdminInfo)
                .WithMany(e => e.Users)
                .HasForeignKey(e => e.AdminInfoId)
                .HasPrincipalKey(e => e.Id)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
