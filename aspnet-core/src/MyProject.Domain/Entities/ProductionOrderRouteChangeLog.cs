using System;
using MyProject.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace MyProject.Entities;

/// <summary>
/// 生产工单工艺路线变更日志
/// 记录修改人、修改时间、修改前后路线版本、修改来源
/// 作为独立聚合根，与ProductionOrder通过id关联
/// </summary>
public class ProductionOrderRouteChangeLog : FullAuditedAggregateRoot<Guid>
{
    /// <summary>关联生产工单id</summary>
    public Guid ProductionOrderId { get; protected set; }

    /// <summary>修改前工艺路线id</summary>
    public Guid? OldProcessRouteId { get; protected set; }

    /// <summary>修改前工艺路线号</summary>
    public string OldProcessRouteNo { get; protected set; }

    /// <summary>修改前工艺路线名称</summary>
    public string OldProcessRouteName { get; protected set; }

    /// <summary>修改后工艺路线id</summary>
    public Guid? NewProcessRouteId { get; protected set; }

    /// <summary>修改后工艺路线号</summary>
    public string NewProcessRouteNo { get; protected set; }

    /// <summary>修改后工艺路线名称</summary>
    public string NewProcessRouteName { get; protected set; }

    /// <summary>修改来源</summary>
    public RouteChangeSource ChangeSource { get; protected set; }

    protected ProductionOrderRouteChangeLog()
    {
        // ORM构造函数
    }

    public ProductionOrderRouteChangeLog(
        Guid id,
        Guid productionOrderId,
        Guid? oldProcessRouteId,
        string oldProcessRouteNo,
        string oldProcessRouteName,
        Guid? newProcessRouteId,
        string newProcessRouteNo,
        string newProcessRouteName,
        RouteChangeSource changeSource
    ) : base(id)
    {
        ProductionOrderId = productionOrderId;
        OldProcessRouteId = oldProcessRouteId;
        OldProcessRouteNo = oldProcessRouteNo;
        OldProcessRouteName = oldProcessRouteName;
        NewProcessRouteId = newProcessRouteId;
        NewProcessRouteNo = newProcessRouteNo;
        NewProcessRouteName = newProcessRouteName;
        ChangeSource = changeSource;
    }
}
