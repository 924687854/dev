using Volo.Abp.Settings;

namespace MesSystem.Settings;

public class MesSystemSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(MesSystemSettings.MySetting1));
    }
}
