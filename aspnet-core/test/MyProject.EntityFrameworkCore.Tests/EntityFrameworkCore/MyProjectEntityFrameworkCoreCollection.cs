using Xunit;

namespace MyProject.EntityFrameworkCore;

[CollectionDefinition(MyProjectTestConsts.CollectionDefinitionName)]
public class MyProjectEntityFrameworkCoreCollection : ICollectionFixture<MyProjectEntityFrameworkCoreFixture>
{

}
