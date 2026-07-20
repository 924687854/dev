using MesSystem.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MesSystem.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MesSystemEntityFrameworkCoreModule),
    typeof(MesSystemApplicationContractsModule)
    )]
public class MesSystemDbMigratorModule : AbpModule
{
}
