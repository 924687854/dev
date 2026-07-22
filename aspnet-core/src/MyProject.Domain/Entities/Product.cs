using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace MyProject.Entities;

/// <summary>
/// 产品主数据
/// Guid 为主键（内码），Code 为业务编码
/// </summary>
public class Product : FullAuditedAggregateRoot<Guid>
{
    /// <summary>产品编码（业务编码，唯一）</summary>
    public string Code { get; protected set; }

    /// <summary>产品名称</summary>
    public string Name { get; protected set; }

    /// <summary>默认单位</summary>
    public string Unit { get; protected set; }

    protected Product()
    {
        // ORM构造函数
    }

    public Product(
        Guid id,
        string code,
        string name,
        string unit
    ) : base(id)
    {
        Code = Check.NotNullOrWhiteSpace(code, nameof(code), maxLength: 64);
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), maxLength: 256);
        Unit = Check.NotNullOrWhiteSpace(unit, nameof(unit), maxLength: 32);
    }
}
