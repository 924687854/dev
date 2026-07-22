using System;
using System.Collections.Generic;
using MyProject.Enums;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace MyProject.Entities;

/// <summary>
/// 生产工单聚合根
/// 采用单头单产品模型，包含工单基本信息、工艺路线关联和ERP源单信息
/// </summary>
public class ProductionOrder : FullAuditedAggregateRoot<Guid>
{
    /// <summary>工单号，MES自生成</summary>
    public string OrderNo { get; protected set; }

    /// <summary>单据状态</summary>
    public ProductionOrderStatus Status { get; protected set; }

    /// <summary>产品编码</summary>
    public string ProductCode { get; protected set; }

    /// <summary>计划数量</summary>
    public decimal PlannedQuantity { get; protected set; }

    /// <summary>单位</summary>
    public string Unit { get; protected set; }

    /// <summary>生产组织</summary>
    public string ProductionOrganization { get; protected set; }

    /// <summary>计划开工日期</summary>
    public DateTime PlannedStartDate { get; protected set; }

    /// <summary>计划完工日期</summary>
    public DateTime? PlannedEndDate { get; protected set; }

    /// <summary>工艺路线id</summary>
    public Guid? ProcessRouteId { get; protected set; }

    /// <summary>工艺路线号</summary>
    public string ProcessRouteNo { get; protected set; }

    /// <summary>工艺路线名称</summary>
    public string ProcessRouteName { get; protected set; }

    /// <summary>ERP源单据id</summary>
    public string ErpSourceId { get; protected set; }

    /// <summary>ERP源单据号</summary>
    public string ErpSourceNo { get; protected set; }

    /// <summary>工序明细行集合</summary>
    public ICollection<ProductionOrderOperation> Operations { get; protected set; }

    protected ProductionOrder()
    {
        // ORM构造函数
        Operations = new List<ProductionOrderOperation>();
    }

    public ProductionOrder(
        Guid id,
        string orderNo,
        string productCode,
        decimal plannedQuantity,
        string unit,
        string productionOrganization,
        DateTime plannedStartDate,
        DateTime? plannedEndDate = null,
        Guid? processRouteId = null,
        string processRouteNo = null,
        string processRouteName = null,
        string erpSourceId = null,
        string erpSourceNo = null
    ) : base(id)
    {
        OrderNo = Check.NotNullOrWhiteSpace(orderNo, nameof(orderNo), maxLength: 64);
        ProductCode = Check.NotNullOrWhiteSpace(productCode, nameof(productCode), maxLength: 64);
        PlannedQuantity = plannedQuantity > 0
            ? plannedQuantity
            : throw new ArgumentOutOfRangeException(nameof(plannedQuantity), "计划数量必须大于0");
        Unit = Check.NotNullOrWhiteSpace(unit, nameof(unit), maxLength: 32);
        ProductionOrganization = Check.NotNullOrWhiteSpace(productionOrganization, nameof(productionOrganization), maxLength: 64);
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        ProcessRouteId = processRouteId;
        ProcessRouteNo = processRouteNo;
        ProcessRouteName = processRouteName;
        ErpSourceId = erpSourceId;
        ErpSourceNo = erpSourceNo;
        Status = ProductionOrderStatus.Released;
        Operations = new List<ProductionOrderOperation>();
    }
}
