using MesSystem.Samples;
using Xunit;

namespace MesSystem.EntityFrameworkCore.Applications;

[Collection(MesSystemTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<MesSystemEntityFrameworkCoreTestModule>
{

}
