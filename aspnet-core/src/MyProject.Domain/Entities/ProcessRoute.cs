using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace MyProject.Entities;

/// <summary>
/// 工艺路线主数据
/// Guid 为主键（内码），RouteNo 为业务编码
/// 支持版本链：同一产品+生产组织下，通过生效/失效日期形成版本链
/// </summary>
public class ProcessRoute : FullAuditedAggregateRoot<Guid>
{
    /// <summary>产品编码</summary>
    public string ProductCode { get; protected set; }

    /// <summary>生产组织编码</summary>
    public string ProductionOrganizationCode { get; protected set; }

    /// <summary>工艺路线号（业务编码）</summary>
    public string RouteNo { get; protected set; }

    /// <summary>工艺路线名称</summary>
    public string RouteName { get; protected set; }

    /// <summary>版本号</summary>
    public int Version { get; protected set; }

    /// <summary>生效日期</summary>
    public DateTime EffectiveDate { get; protected set; }

    /// <summary>失效日期（9999-12-31 表示永不过期）</summary>
    public DateTime ExpiryDate { get; protected set; }

    /// <summary>工序集合</summary>
    public ICollection<ProcessRouteOperation> Operations { get; protected set; }

    protected ProcessRoute()
    {
        // ORM构造函数
        Operations = new List<ProcessRouteOperation>();
    }

    public ProcessRoute(
        Guid id,
        string productCode,
        string productionOrganizationCode,
        string routeNo,
        string routeName,
        int version,
        DateTime effectiveDate,
        DateTime? expiryDate = null
    ) : base(id)
    {
        ProductCode = Check.NotNullOrWhiteSpace(productCode, nameof(productCode), maxLength: 64);
        ProductionOrganizationCode = Check.NotNullOrWhiteSpace(productionOrganizationCode, nameof(productionOrganizationCode), maxLength: 64);
        RouteNo = Check.NotNullOrWhiteSpace(routeNo, nameof(routeNo), maxLength: 64);
        RouteName = Check.NotNullOrWhiteSpace(routeName, nameof(routeName), maxLength: 256);
        Version = version > 0
            ? version
            : throw new ArgumentOutOfRangeException(nameof(version), "版本号必须大于0");
        EffectiveDate = effectiveDate;
        ExpiryDate = expiryDate ?? new DateTime(9999, 12, 31);
        Operations = new List<ProcessRouteOperation>();
    }
}
