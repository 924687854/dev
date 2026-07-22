namespace MyProject.Enums;

/// <summary>
/// 生产工单状态枚举
/// 正向流转：已下达 → 已分解 → 进行中 → 已完成
/// 回退受工序明细行关联规则约束
/// </summary>
public enum ProductionOrderStatus
{
    /// <summary>已下达：建单成功后的初始状态</summary>
    Released = 0,

    /// <summary>已分解：分解生成工序明细行成功</summary>
    Decomposed = 1,

    /// <summary>进行中：首道工序明细行开工</summary>
    InProgress = 2,

    /// <summary>已完成：全部工序明细行完工</summary>
    Completed = 3
}
