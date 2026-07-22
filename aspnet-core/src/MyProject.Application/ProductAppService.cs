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
/// 产品主数据应用服务，继承 ABP 标准 CrudAppService
/// </summary>
public class ProductAppService :
    CrudAppService<Product, ProductDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateProductDto>,
    IProductAppService
{
    public ProductAppService(IRepository<Product, Guid> repository)
        : base(repository)
    {
        GetPolicyName = MyProjectPermissions.Product.Default;
        GetListPolicyName = MyProjectPermissions.Product.Default;
        CreatePolicyName = MyProjectPermissions.Product.Create;
        UpdatePolicyName = MyProjectPermissions.Product.Update;
        DeletePolicyName = MyProjectPermissions.Product.Delete;
    }
}
