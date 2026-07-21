using Microsoft.Extensions.Localization;
using MyProject.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MyProject;

[Dependency(ReplaceServices = true)]
public class MyProjectBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<MyProjectResource> _localizer;

    public MyProjectBrandingProvider(IStringLocalizer<MyProjectResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
