using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace MyProject.Dtos;

/// <summary>生产组织输出 DTO</summary>
public class ProductionOrganizationDto : EntityDto<Guid>
{
    public string Code { get; set; }
    public string Name { get; set; }
}

/// <summary>生产组织创建/更新 DTO</summary>
public class CreateUpdateProductionOrganizationDto
{
    [Required]
    [StringLength(64)]
    public string Code { get; set; }

    [Required]
    [StringLength(256)]
    public string Name { get; set; }
}
