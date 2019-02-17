﻿using DemoCore.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoCore.Infra.Data.EntityConfigs
{
    public class DeveloperConfiguration: BaseConfiguration<Developer>
    {
        public override void Configure(EntityTypeBuilder<Developer> builder)
        {
            builder.HasKey(x => x.Id).ForSqlServerIsClustered();
            builder.Property(x => x.Id).UseSqlServerIdentityColumn().ValueGeneratedOnAdd();

            builder.Property(x => x.DescriptionEN).HasColumnName("DescriptionEN").HasColumnType("nvarchar(500)").IsRequired();
            builder.Property(x => x.DescriptionPT).HasColumnName("DescriptionPT").HasColumnType("nvarchar(500)").IsRequired();

            base.Configure(builder);
        }
    }
}