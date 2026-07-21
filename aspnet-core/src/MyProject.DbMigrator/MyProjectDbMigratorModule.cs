using MyProject.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MyProject.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MyProjectEntityFrameworkCoreModule),
    typeof(MyProjectApplicationContractsModule)
    )]
public class MyProjectDbMigratorModule : AbpModule
{
}
