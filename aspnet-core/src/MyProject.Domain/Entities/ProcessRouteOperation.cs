using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace MyProject.Entities;

/// <summary>
/// 工艺路线工序
/// 作为 ProcessRoute 聚合内子实体，通过聚合根访问
/// </summary>
public class ProcessRouteOperation : Entity<Guid>
{
    /// <summary>工序序号</summary>
    public int SequenceNumber { get; protected set; }

    /// <summary>工序编码（主数据中的工序编码）</summary>
    public string OperationCode { get; protected set; }

    /// <summary>工序名称</summary>
    public string OperationName { get; protected set; }

    /// <summary>所属工艺路线id（外键）</summary>
    public Guid ProcessRouteId { get; protected set; }

    protected ProcessRouteOperation()
    {
        // ORM构造函数
    }

    public ProcessRouteOperation(
        Guid id,
        Guid processRouteId,
        int sequenceNumber,
        string operationCode,
        string operationName
    ) : base(id)
    {
        ProcessRouteId = processRouteId;
        SequenceNumber = sequenceNumber > 0
            ? sequenceNumber
            : throw new ArgumentOutOfRangeException(nameof(sequenceNumber), "工序序号必须大于0");
        OperationCode = Check.NotNullOrWhiteSpace(operationCode, nameof(operationCode), maxLength: 64);
        OperationName = Check.NotNullOrWhiteSpace(operationName, nameof(operationName), maxLength: 256);
    }
}
