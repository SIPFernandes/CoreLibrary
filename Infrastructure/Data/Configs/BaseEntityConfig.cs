using CoreLibrary.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CoreLibrary.Infrastructure.Data.Configs
{
    public class BaseEntityConfig : IEntityTypeConfiguration<BaseEntity>
    {
        public void Configure(EntityTypeBuilder<BaseEntity> builder)
        {
            builder.UseTpcMappingStrategy();

            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.Property(b => b.CreatedAt).HasDefaultValueSql("getutcdate()");
            builder.Property(b => b.ModifiedAt).HasDefaultValueSql("getutcdate()");
        }
    }
}
