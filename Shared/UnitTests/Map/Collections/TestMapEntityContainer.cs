using Bitspoke.Ludus.Shared.Environment.Map.Entities.Components;
using NUnit.Framework;

namespace Bitspoke.Ludus.Shared.UnitTests.Map.Collections;

[TestFixture]
public class TestMapEntityContainer
{
    public MapEntityContainerComponent EntityContainer { get; set; }
    
    [SetUp]
    public void SetUp()
    {
        // MapInitConfig config = new MapInitConfig();
        // Environment.Map.Map map = new Environment.Map.Map(config);
        // EntityContainer = new MapEntityContainerComponent(map.MapID);

    }

    [Test]
    [Order(1)]
    public void TestAddEntity()
    {
        
    }
    
    [Test]
    [Order(2)]
    public void TestUpdateEntity()
    {
        
    }

    [Test]
    [Order(3)]
    public void TestRemoveEntity()
    {
        
    }

}