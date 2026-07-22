using System;
using MyProject.Enums;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace MyProject.Entities;

/// <summary>
/// 生产工单工序明细行
/// 作为ProductionOrder聚合内子实体，通过聚合根访问
/// 数据为分解时从工艺路线复制的快照，生成后与主数据脱钩
/// </summary>
public class ProductionOrderOperation : Entity<Guid>
{
    /// <summary>工序识别码，全场唯一，作为工序执行实例的唯一标识</summary>
    public string OperationCode { get; protected set; }

    /// <summary>工序号/序号</summary>
    public int SequenceNumber { get; protected set; }

    /// <summary>工序名称</summary>
    public string OperationName { get; protected set; }

    /// <summary>行状态</summary>
    public ProductionOrderOperationStatus Status { get; protected set; }

    /// <summary>计划数量</summary>
    public decimal PlannedQuantity { get; protected set; }

    /// <summary>计划开工日期</summary>
    public DateTime PlannedStartDate { get; protected set; }

    /// <summary>计划完工日期</summary>
    public DateTime? PlannedEndDate { get; protected set; }

    /// <summary>所属生产工单id（外键）</summary>
    public Guid ProductionOrderId { get; protected set; }

    protected ProductionOrderOperation()
    {
        // ORM构造函数
    }

    public ProductionOrderOperation(
        Guid id,
        Guid productionOrderId,
        string operationCode,
        int sequenceNumber,
        string operationName,
        decimal plannedQuantity,
        DateTime plannedStartDate,
        DateTime? plannedEndDate = null
    ) : base(id)
    {
        ProductionOrderId = productionOrderId;
        OperationCode = Check.NotNullOrWhiteSpace(operationCode, nameof(operationCode), maxLength: 64);
        SequenceNumber = sequenceNumber > 0
            ? sequenceNumber
            : throw new ArgumentOutOfRangeException(nameof(sequenceNumber), "工序号必须大于0");
        OperationName = Check.NotNullOrWhiteSpace(operationName, nameof(operationName), maxLength: 256);
        PlannedQuantity = plannedQuantity > 0
            ? plannedQuantity
            : throw new ArgumentOutOfRangeException(nameof(plannedQuantity), "计划数量必须大于0");
        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
        Status = ProductionOrderOperationStatus.NotStarted;
    }
}
