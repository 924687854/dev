using System;
using MyProject.Dtos;
using MyProject.Entities;
using MyProject.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace MyProject;

/// <summary>
/// 生产组织主数据应用服务，继承 ABP 标准 CrudAppService
/// </summary>
public class ProductionOrganizationAppService :
    CrudAppService<ProductionOrganization, ProductionOrganizationDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateProductionOrganizationDto>,
    IProductionOrganizationAppService
{
    public ProductionOrganizationAppService(IRepository<ProductionOrganization, Guid> repository)
        : base(repository)
    {
        GetPolicyName = MyProjectPermissions.ProductionOrganization.Default;
        GetListPolicyName = MyProjectPermissions.ProductionOrganization.Default;
        CreatePolicyName = MyProjectPermissions.ProductionOrganization.Create;
        UpdatePolicyName = MyProjectPermissions.ProductionOrganization.Update;
        DeletePolicyName = MyProjectPermissions.ProductionOrganization.Delete;
    }
}
