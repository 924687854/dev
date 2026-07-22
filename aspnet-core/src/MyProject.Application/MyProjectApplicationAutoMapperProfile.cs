using AutoMapper;
using MyProject.Dtos;
using MyProject.Entities;

namespace MyProject;

public class MyProjectApplicationAutoMapperProfile : Profile
{
    public MyProjectApplicationAutoMapperProfile()
    {
        // 生产工单
        CreateMap<ProductionOrder, ProductionOrderDto>();
        CreateMap<ProductionOrderOperation, ProductionOrderOperationDto>();

        // 产品主数据
        CreateMap<Product, ProductDto>();
        CreateMap<CreateUpdateProductDto, Product>()
            .ForMember(x => x.Id, opt => opt.Ignore());

        // 生产组织主数据
        CreateMap<ProductionOrganization, ProductionOrganizationDto>();
        CreateMap<CreateUpdateProductionOrganizationDto, ProductionOrganization>()
            .ForMember(x => x.Id, opt => opt.Ignore());

        // 工艺路线主数据
        CreateMap<ProcessRoute, ProcessRouteDto>();
        CreateMap<ProcessRouteOperation, ProcessRouteOperationDto>();
        CreateMap<CreateUpdateProcessRouteDto, ProcessRoute>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.Operations, opt => opt.Ignore());
        CreateMap<CreateUpdateProcessRouteOperationDto, ProcessRouteOperation>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.ProcessRouteId, opt => opt.Ignore());
    }
}
