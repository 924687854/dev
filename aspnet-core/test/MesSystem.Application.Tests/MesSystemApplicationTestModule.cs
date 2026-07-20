using Volo.Abp.Modularity;

namespace MesSystem;

[DependsOn(
    typeof(MesSystemApplicationModule),
    typeof(MesSystemDomainTestModule)
)]
public class MesSystemApplicationTestModule : AbpModule
{

}
