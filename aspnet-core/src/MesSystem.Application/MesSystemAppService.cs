using System;
using System.Collections.Generic;
using System.Text;
using MesSystem.Localization;
using Volo.Abp.Application.Services;

namespace MesSystem;

/* Inherit your application services from this class.
 */
public abstract class MesSystemAppService : ApplicationService
{
    protected MesSystemAppService()
    {
        LocalizationResource = typeof(MesSystemResource);
    }
}
