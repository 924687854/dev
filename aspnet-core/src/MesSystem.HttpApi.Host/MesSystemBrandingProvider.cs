using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MesSystem;

[Dependency(ReplaceServices = true)]
public class MesSystemBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "MesSystem";
}
