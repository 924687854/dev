using MyProject.Localization;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace MyProject;

[DependsOn(
    typeof(AbpAuditLoggingDomainSharedModule),//审计追溯
    typeof(AbpBackgroundJobsDomainSharedModule),//后台异步任务
    typeof(AbpFeatureManagementDomainSharedModule),//版本功能开关，租户相关
    typeof(AbpIdentityDomainSharedModule),//AbpUserConsts、IdentityRoleConsts（比如用户名最大长度等常量）、身份相关错误码和本地化。
    typeof(AbpOpenIddictDomainSharedModule),//登录认证
    typeof(AbpPermissionManagementDomainSharedModule),//权限存储授权
    typeof(AbpSettingManagementDomainSharedModule),//可配置参数
    typeof(AbpTenantManagementDomainSharedModule)    //租户管理
    )]
public class MyProjectDomainSharedModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        MyProjectGlobalFeatureConfigurator.Configure();//全局功能开关
        MyProjectModuleExtensionConfigurator.Configure(); // 实体扩展
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // (1) 注册虚拟文件系统，让嵌入的 json 可被读取
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<MyProjectDomainSharedModule>();
        });

// (2) 注册本地化资源
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<MyProjectResource>("en")// 默认语言 en
                .AddBaseTypes(typeof(AbpValidationResource)) // 继承验证消息本地化
                .AddVirtualJson("/Localization/MyProject"); // json 文件路径

            options.DefaultResourceType = typeof(MyProjectResource);// 全局默认资源
        });
// (3) 把 "MyProject" 命名空间的异常错误码映射到本地化资源。它让你抛出的 BusinessException("MyProject:xxx") 能自动去 json 里找对应的中/英文提示。
        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("MyProject", typeof(MyProjectResource));
        });
    }
}
