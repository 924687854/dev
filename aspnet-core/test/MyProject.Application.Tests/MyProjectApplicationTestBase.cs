using Volo.Abp.Modularity;

namespace MyProject;

public abstract class MyProjectApplicationTestBase<TStartupModule> : MyProjectTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
