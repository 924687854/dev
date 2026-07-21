using Volo.Abp.Modularity;

namespace MyProject;

[DependsOn(
    typeof(MyProjectDomainModule),
    typeof(MyProjectTestBaseModule)
)]
public class MyProjectDomainTestModule : AbpModule
{

}
