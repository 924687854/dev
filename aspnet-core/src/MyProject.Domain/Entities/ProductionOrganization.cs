using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace MyProject.Entities;

/// <summary>
/// 生产组织主数据
/// Guid 为主键（内码），Code 为业务编码
/// </summary>
public class ProductionOrganization : FullAuditedAggregateRoot<Guid>
{
    /// <summary>组织编码（业务编码，唯一）</summary>
    public string Code { get; protected set; }

    /// <summary>组织名称</summary>
    public string Name { get; protected set; }

    protected ProductionOrganization()
    {
        // ORM构造函数
    }

    public ProductionOrganization(
        Guid id,
        string code,
        string name
    ) : base(id)
    {
        Code = Check.NotNullOrWhiteSpace(code, nameof(code), maxLength: 64);
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: 256);
    }
}
