using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace MyProject.Dtos;

/// <summary>工艺路线输出 DTO</summary>
public class ProcessRouteDto : EntityDto<Guid>
{
    public string ProductCode { get; set; }
    public string ProductionOrganizationCode { get; set; }
    public string RouteNo { get; set; }
    public string RouteName { get; set; }
    public int Version { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public List<ProcessRouteOperationDto> Operations { get; set; } = new();
}

/// <summary>工艺路线工序输出 DTO</summary>
public class ProcessRouteOperationDto
{
    public Guid Id { get; set; }
    public int SequenceNumber { get; set; }
    public string OperationCode { get; set; }
    public string OperationName { get; set; }
}

/// <summary>工艺路线创建/更新 DTO</summary>
public class CreateUpdateProcessRouteDto
{
    [Required]
    [StringLength(64)]
    public string ProductCode { get; set; }

    [Required]
    [StringLength(64)]
    public string ProductionOrganizationCode { get; set; }

    [Required]
    [StringLength(64)]
    public string RouteNo { get; set; }

    [Required]
    [StringLength(256)]
    public string RouteName { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Version { get; set; }

    [Required]
    public DateTime EffectiveDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    [Required]
    public List<CreateUpdateProcessRouteOperationDto> Operations { get; set; } = new();
}

/// <summary>工艺路线工序创建/更新 DTO</summary>
public class CreateUpdateProcessRouteOperationDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int SequenceNumber { get; set; }

    [Required]
    [StringLength(64)]
    public string OperationCode { get; set; }

    [Required]
    [StringLength(256)]
    public string OperationName { get; set; }
}
