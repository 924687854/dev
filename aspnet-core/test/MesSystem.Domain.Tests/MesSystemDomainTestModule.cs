using Volo.Abp.Modularity;

namespace MesSystem;

[DependsOn(
    typeof(MesSystemDomainModule),
    typeof(MesSystemTestBaseModule)
)]
public class MesSystemDomainTestModule : AbpModule
{

}
