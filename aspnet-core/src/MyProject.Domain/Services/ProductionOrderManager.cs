using System;
using System.Linq;
using System.Threading.Tasks;
using MyProject.Entities;
using MyProject.Enums;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.DependencyInjection;

namespace MyProject.Services;

/// <summary>
/// 生产工单领域服务
/// 封装创建生产工单的核心业务规则：校验→查主数据→匹配工艺路线→生成工单号→创建实体
/// 手工创建与ERP接口创建共用此服务
/// </summary>
public class ProductionOrderManager : DomainService
{
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly IRepository<ProductionOrganization, Guid> _orgRepository;
    private readonly IRepository<ProcessRoute, Guid> _processRouteRepository;
    private readonly IOrderNumberGenerator _orderNumberGenerator;

    public ProductionOrderManager(
        IRepository<Product, Guid> productRepository,
        IRepository<ProductionOrganization, Guid> orgRepository,
        IRepository<ProcessRoute, Guid> processRouteRepository,
        IOrderNumberGenerator orderNumberGenerator)
    {
        _productRepository = productRepository;
        _orgRepository = orgRepository;
        _processRouteRepository = processRouteRepository;
        _orderNumberGenerator = orderNumberGenerator;
    }

    /// <summary>
    /// 创建生产工单
    /// 统一入口，手工与ERP共用
    /// </summary>
    public async Task<ProductionOrder> CreateAsync(
        string productCode,
        decimal plannedQuantity,
        string unit,
        string productionOrganization,
        DateTime plannedStartDate,
        DateTime? plannedEndDate = null,
        string erpSourceId = null,
        string erpSourceNo = null)
    {
        // 1. 基础校验
        if (plannedQuantity <= 0)
        {
            throw new BusinessException("MyProject:ProductionOrder:InvalidQuantity")
                .WithData("Quantity", plannedQuantity);
        }

        if (plannedEndDate.HasValue && plannedEndDate.Value < plannedStartDate)
        {
            throw new BusinessException("MyProject:ProductionOrder:InvalidDateRange")
                .WithData("StartDate", plannedStartDate)
                .WithData("EndDate", plannedEndDate.Value);
        }

        // 2. 产品编码存在性校验
        var productQueryable = await _productRepository.GetQueryableAsync();
        var productExists = productQueryable.Any(x => x.Code == productCode);
        if (!productExists)
        {
            throw new BusinessException("MyProject:ProductionOrder:ProductNotFound")
                .WithData("ProductCode", productCode);
        }

        // 3. 生产组织存在性校验
        var orgQueryable = await _orgRepository.GetQueryableAsync();
        var orgExists = orgQueryable.Any(x => x.Code == productionOrganization);
        if (!orgExists)
        {
            throw new BusinessException("MyProject:ProductionOrder:OrganizationNotFound")
                .WithData("OrganizationCode", productionOrganization);
        }

        // 4. ERP入口额外必填校验
        if (!string.IsNullOrWhiteSpace(erpSourceId) && string.IsNullOrWhiteSpace(erpSourceNo))
        {
            throw new BusinessException("MyProject:ProductionOrder:ErpSourceNoRequired");
        }

        // 5. 自动匹配工艺路线（产品编码 + 生产组织 + 当天生效）
        var routeQueryable = await _processRouteRepository.GetQueryableAsync();
        var today = Clock.Now.Date;
        var matchedRoute = routeQueryable
            .Where(x => x.ProductCode == productCode
                     && x.ProductionOrganizationCode == productionOrganization
                     && x.EffectiveDate <= today
                     && x.ExpiryDate >= today)
            .FirstOrDefault();

        // 6. 生成工单号
        var orderNo = await _orderNumberGenerator.GenerateAsync();

        // 7. 创建实体
        var order = new ProductionOrder(
            id: GuidGenerator.Create(),
            orderNo: orderNo,
            productCode: productCode,
            plannedQuantity: plannedQuantity,
            unit: unit,
            productionOrganization: productionOrganization,
            plannedStartDate: plannedStartDate,
            plannedEndDate: plannedEndDate,
            processRouteId: matchedRoute?.Id,
            processRouteNo: matchedRoute?.RouteNo,
            processRouteName: matchedRoute?.RouteName,
            erpSourceId: erpSourceId,
            erpSourceNo: erpSourceNo
        );

        return order;
    }
}
