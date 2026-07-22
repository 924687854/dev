using MyProject.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace MyProject.Permissions;

public class MyProjectPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(MyProjectPermissions.GroupName,
            L("Permission:MyProject"));

        // 生产工单
        var productionOrder = myGroup.AddPermission(
            MyProjectPermissions.ProductionOrder.Default,
            L("Permission:ProductionOrder"));
        productionOrder.AddChild(
            MyProjectPermissions.ProductionOrder.Create,
            L("Permission:ProductionOrder.Create"));
        productionOrder.AddChild(
            MyProjectPermissions.ProductionOrder.Read,
            L("Permission:ProductionOrder.Read"));

        // 产品主数据
        var product = myGroup.AddPermission(
            MyProjectPermissions.Product.Default,
            L("Permission:Product"));
        product.AddChild(
            MyProjectPermissions.Product.Create,
            L("Permission:Product.Create"));
        product.AddChild(
            MyProjectPermissions.Product.Update,
            L("Permission:Product.Update"));
        product.AddChild(
            MyProjectPermissions.Product.Delete,
            L("Permission:Product.Delete"));

        // 生产组织主数据
        var org = myGroup.AddPermission(
            MyProjectPermissions.ProductionOrganization.Default,
            L("Permission:ProductionOrganization"));
        org.AddChild(
            MyProjectPermissions.ProductionOrganization.Create,
            L("Permission:ProductionOrganization.Create"));
        org.AddChild(
            MyProjectPermissions.ProductionOrganization.Update,
            L("Permission:ProductionOrganization.Update"));
        org.AddChild(
            MyProjectPermissions.ProductionOrganization.Delete,
            L("Permission:ProductionOrganization.Delete"));

        // 工艺路线主数据
        var processRoute = myGroup.AddPermission(
            MyProjectPermissions.ProcessRoute.Default,
            L("Permission:ProcessRoute"));
        processRoute.AddChild(
            MyProjectPermissions.ProcessRoute.Create,
            L("Permission:ProcessRoute.Create"));
        processRoute.AddChild(
            MyProjectPermissions.ProcessRoute.Update,
            L("Permission:ProcessRoute.Update"));
        processRoute.AddChild(
            MyProjectPermissions.ProcessRoute.Delete,
            L("Permission:ProcessRoute.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<MyProjectResource>(name);
    }
}
