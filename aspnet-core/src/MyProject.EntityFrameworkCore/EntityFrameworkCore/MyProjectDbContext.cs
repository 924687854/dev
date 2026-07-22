using Microsoft.EntityFrameworkCore;
using MyProject.Entities;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace MyProject.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class MyProjectDbContext :
    AbpDbContext<MyProjectDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    // MES 生产工单
    public DbSet<ProductionOrder> ProductionOrders { get; set; }
    public DbSet<ProductionOrderOperation> ProductionOrderOperations { get; set; }
    public DbSet<ProductionOrderRouteChangeLog> ProductionOrderRouteChangeLogs { get; set; }

    // 主数据
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductionOrganization> ProductionOrganizations { get; set; }
    public DbSet<ProcessRoute> ProcessRoutes { get; set; }
    public DbSet<ProcessRouteOperation> ProcessRouteOperations { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public MyProjectDbContext(DbContextOptions<MyProjectDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        // 生产工单
        builder.Entity<ProductionOrder>(b =>
        {
            b.ToTable(MyProjectConsts.DbTablePrefix + "ProductionOrders");
            b.ConfigureByConvention();

            b.Property(x => x.OrderNo).IsRequired().HasMaxLength(64);
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.ProductCode).IsRequired().HasMaxLength(64);
            b.Property(x => x.PlannedQuantity).IsRequired().HasPrecision(18, 4);
            b.Property(x => x.Unit).IsRequired().HasMaxLength(32);
            b.Property(x => x.ProductionOrganization).IsRequired().HasMaxLength(64);
            b.Property(x => x.PlannedStartDate).IsRequired();
            b.Property(x => x.PlannedEndDate);
            b.Property(x => x.ProcessRouteId);
            b.Property(x => x.ProcessRouteNo).IsRequired(false).HasMaxLength(64);
            b.Property(x => x.ProcessRouteName).IsRequired(false).HasMaxLength(256);
            b.Property(x => x.ErpSourceId).IsRequired(false).HasMaxLength(128);
            b.Property(x => x.ErpSourceNo).IsRequired(false).HasMaxLength(128);

            // 工单号唯一索引
            b.HasIndex(x => x.OrderNo).IsUnique();
            // ERP源单据id唯一索引（MySQL下UNIQUE索引天然允许多个NULL值，无需额外过滤条件）
            b.HasIndex(x => x.ErpSourceId).IsUnique();

            b.HasMany(x => x.Operations)
                .WithOne()
                .HasForeignKey(x => x.ProductionOrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 工序明细行
        builder.Entity<ProductionOrderOperation>(b =>
        {
            b.ToTable(MyProjectConsts.DbTablePrefix + "ProductionOrderOperations");
            b.ConfigureByConvention();

            b.Property(x => x.OperationCode).IsRequired().HasMaxLength(64);
            b.Property(x => x.SequenceNumber).IsRequired();
            b.Property(x => x.OperationName).IsRequired().HasMaxLength(256);
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.PlannedQuantity).IsRequired().HasPrecision(18, 4);
            b.Property(x => x.PlannedStartDate).IsRequired();
            b.Property(x => x.PlannedEndDate);
            b.Property(x => x.ProductionOrderId).IsRequired();

            // 工序识别码全场唯一
            b.HasIndex(x => x.OperationCode).IsUnique();
        });

        // 工艺路线变更日志
        builder.Entity<ProductionOrderRouteChangeLog>(b =>
        {
            b.ToTable(MyProjectConsts.DbTablePrefix + "ProductionOrderRouteChangeLogs");
            b.ConfigureByConvention();

            b.Property(x => x.ProductionOrderId).IsRequired();
            b.Property(x => x.OldProcessRouteId);
            b.Property(x => x.OldProcessRouteNo).IsRequired(false).HasMaxLength(64);
            b.Property(x => x.OldProcessRouteName).IsRequired(false).HasMaxLength(256);
            b.Property(x => x.NewProcessRouteId);
            b.Property(x => x.NewProcessRouteNo).IsRequired(false).HasMaxLength(64);
            b.Property(x => x.NewProcessRouteName).IsRequired(false).HasMaxLength(256);
            b.Property(x => x.ChangeSource).IsRequired();

            b.HasIndex(x => x.ProductionOrderId);
        });

        // 产品主数据
        builder.Entity<Product>(b =>
        {
            b.ToTable(MyProjectConsts.DbTablePrefix + "Products");
            b.ConfigureByConvention();

            b.Property(x => x.Code).IsRequired().HasMaxLength(64);
            b.Property(x => x.Name).IsRequired().HasMaxLength(256);
            b.Property(x => x.Unit).IsRequired().HasMaxLength(32);

            // 产品编码唯一
            b.HasIndex(x => x.Code).IsUnique();
        });

        // 生产组织主数据
        builder.Entity<ProductionOrganization>(b =>
        {
            b.ToTable(MyProjectConsts.DbTablePrefix + "ProductionOrganizations");
            b.ConfigureByConvention();

            b.Property(x => x.Code).IsRequired().HasMaxLength(64);
            b.Property(x => x.Name).IsRequired().HasMaxLength(256);

            // 组织编码唯一
            b.HasIndex(x => x.Code).IsUnique();
        });

        // 工艺路线主数据
        builder.Entity<ProcessRoute>(b =>
        {
            b.ToTable(MyProjectConsts.DbTablePrefix + "ProcessRoutes");
            b.ConfigureByConvention();

            b.Property(x => x.ProductCode).IsRequired().HasMaxLength(64);
            b.Property(x => x.ProductionOrganizationCode).IsRequired().HasMaxLength(64);
            b.Property(x => x.RouteNo).IsRequired().HasMaxLength(64);
            b.Property(x => x.RouteName).IsRequired().HasMaxLength(256);
            b.Property(x => x.Version).IsRequired();
            b.Property(x => x.EffectiveDate).IsRequired();
            b.Property(x => x.ExpiryDate).IsRequired();

            // 产品+组织+版本 唯一
            b.HasIndex(x => new { x.ProductCode, x.ProductionOrganizationCode, x.Version }).IsUnique();
            // 工艺路线号唯一
            b.HasIndex(x => x.RouteNo).IsUnique();

            b.HasMany(x => x.Operations)
                .WithOne()
                .HasForeignKey(x => x.ProcessRouteId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 工艺路线工序
        builder.Entity<ProcessRouteOperation>(b =>
        {
            b.ToTable(MyProjectConsts.DbTablePrefix + "ProcessRouteOperations");
            b.ConfigureByConvention();

            b.Property(x => x.SequenceNumber).IsRequired();
            b.Property(x => x.OperationCode).IsRequired().HasMaxLength(64);
            b.Property(x => x.OperationName).IsRequired().HasMaxLength(256);
            b.Property(x => x.ProcessRouteId).IsRequired();
        });
    }
}
