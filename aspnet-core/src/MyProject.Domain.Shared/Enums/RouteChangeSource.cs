namespace MyProject.Enums;

/// <summary>
/// 工艺路线变更来源枚举
/// </summary>
public enum RouteChangeSource
{
    /// <summary>手工：用户手动修改工艺路线</summary>
    Manual = 0,

    /// <summary>接口：通过ERP接口修改工艺路线</summary>
    Interface = 1
}
