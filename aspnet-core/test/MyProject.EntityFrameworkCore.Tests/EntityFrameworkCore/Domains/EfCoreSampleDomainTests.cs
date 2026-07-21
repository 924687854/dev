using MyProject.Samples;
using Xunit;

namespace MyProject.EntityFrameworkCore.Domains;

[Collection(MyProjectTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<MyProjectEntityFrameworkCoreTestModule>
{

}
