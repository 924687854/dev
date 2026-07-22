using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace MyProject.Dtos;

/// <summary>产品输出 DTO</summary>
public class ProductDto : EntityDto<Guid>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Unit { get; set; }
}

/// <summary>产品创建/更新 DTO</summary>
public class CreateUpdateProductDto
{
    [Required]
    [StringLength(64)]
    public string Code { get; set; }

    [Required]
    [StringLength(256)]
    public string Name { get; set; }

    [Required]
    [StringLength(32)]
    public string Unit { get; set; }
}
