using System;
using System.ComponentModel.DataAnnotations;

namespace MyProject.Dtos;

/// <summary>ERP 接口创建生产工单输入 DTO</summary>
public class CreateProductionOrderFromErpDto
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

    [Required]
    [StringLength(128)]
    public string ErpSourceId { get; set; }

    [Required]
    [StringLength(128)]
    public string ErpSourceNo { get; set; }
}
