namespace MyProject.Enums;

/// <summary>
/// 生产工单工序明细行状态枚举
/// 流转：未开工 → 已开工 → 完工
/// </summary>
public enum ProductionOrderOperationStatus
{
    /// <summary>未开工：工序明细行初始状态</summary>
    NotStarted = 0,

    /// <summary>已开工：工序已开始执行</summary>
    Started = 1,

    /// <summary>完工：工序已执行完成</summary>
    Completed = 2
}
