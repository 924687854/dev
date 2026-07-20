using MesSystem.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MesSystem.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MesSystemController : AbpControllerBase
{
    protected MesSystemController()
    {
        LocalizationResource = typeof(MesSystemResource);
    }
}
