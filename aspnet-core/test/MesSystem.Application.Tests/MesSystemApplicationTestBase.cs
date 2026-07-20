using Volo.Abp.Modularity;

namespace MesSystem;

public abstract class MesSystemApplicationTestBase<TStartupModule> : MesSystemTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
