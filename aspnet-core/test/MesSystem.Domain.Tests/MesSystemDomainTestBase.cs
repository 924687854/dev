using Volo.Abp.Modularity;

namespace MesSystem;

/* Inherit from this class for your domain layer tests. */
public abstract class MesSystemDomainTestBase<TStartupModule> : MesSystemTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
