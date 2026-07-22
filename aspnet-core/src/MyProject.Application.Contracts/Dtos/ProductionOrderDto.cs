using System;
using System.ComponentModel.DataAnnotations;
using MyProject.Enums;

namespace MyProject.Dtos;

/// <summary>生产工单输出 DTO</summary>
public class ProductionOrderDto
{
    public Guid Id { get; set; }
    public string OrderNo { get; set; }
    public ProductionOrderStatus Status { get; set; }
    public string ProductCode { get; set; }
    public decimal PlannedQuantity { get; set; }
    public string Unit { get; set; }
    public string ProductionOrganization { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public Guid? ProcessRouteId { get; set; }
    public string ProcessRouteNo { get; set; }
    public string ProcessRouteName { get; set; }
    public string ErpSourceId { get; set; }
    public string ErpSourceNo { get; set; }
}

/// <summary>生产工单工序明细行输出 DTO</summary>
public class ProductionOrderOperationDto
{
    public Guid Id { get; set; }
    public string OperationCode { get; set; }
    public int SequenceNumber { get; set; }
    public string OperationName { get; set; }
    public ProductionOrderOperationStatus Status { get; set; }
    public decimal PlannedQuantity { get; set; }
    public DateTime PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
}
