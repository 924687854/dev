using MesSystem.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace MesSystem.Permissions;

public class MesSystemPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(MesSystemPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(MesSystemPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MesSystemResource>(name);
    }
}
