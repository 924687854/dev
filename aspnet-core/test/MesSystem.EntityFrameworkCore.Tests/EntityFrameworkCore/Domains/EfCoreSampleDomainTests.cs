using MesSystem.Samples;
using Xunit;

namespace MesSystem.EntityFrameworkCore.Domains;

[Collection(MesSystemTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<MesSystemEntityFrameworkCoreTestModule>
{

}
