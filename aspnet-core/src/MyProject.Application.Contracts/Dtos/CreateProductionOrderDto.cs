using System;
using System.ComponentModel.DataAnnotations;

namespace MyProject.Dtos;

/// <summary>手工创建生产工单输入 DTO</summary>
public class CreateProductionOrderDto
{
    [Required]
    [StringLength(64)]
    public string ProductCode { get; set; }

    [Required]
    [Range(0.0001, double.MaxValue)]
    public decimal PlannedQuantity { get; set; }

    [Required]
    [StringLength(32)]
    public string Unit { get; set; }

    [Required]
    [StringLength(64)]
    public string ProductionOrganization { get; set; }

    [Required]
    public DateTime PlannedStartDate { get; set; }

    public DateTime? PlannedEndDate { get; set; }
}
