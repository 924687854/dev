namespace MyProject.Permissions;

/// <summary>
/// 权限名称常量，遵循 ABP 规范：GroupName + 子节点
/// </summary>
public static class MyProjectPermissions
{
    public const string GroupName = "MyProject";

    // 生产工单
    public static class ProductionOrder
    {
        public const string Default = GroupName + ".ProductionOrder";
        public const string Create = Default + ".Create";
        public const string Read = Default + ".Read";
    }

    // 产品主数据
    public static class Product
    {
        public const string Default = GroupName + ".Product";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    // 生产组织主数据
    public static class ProductionOrganization
    {
        public const string Default = GroupName + ".ProductionOrganization";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    // 工艺路线主数据
    public static class ProcessRoute
    {
        public const string Default = GroupName + ".ProcessRoute";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }
}
