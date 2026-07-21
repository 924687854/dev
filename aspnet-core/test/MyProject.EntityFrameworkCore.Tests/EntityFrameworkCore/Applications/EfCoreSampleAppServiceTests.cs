using MyProject.Samples;
using Xunit;

namespace MyProject.EntityFrameworkCore.Applications;

[Collection(MyProjectTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<MyProjectEntityFrameworkCoreTestModule>
{

}
