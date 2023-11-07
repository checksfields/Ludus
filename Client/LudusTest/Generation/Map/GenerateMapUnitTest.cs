using Bitspoke.Ludus.Shared.Environment.Map;
using Bitspoke.Ludus.Shared.Environment.Map.Generation;
using Bitspoke.Ludus.Shared.Environment.World;
using Godot;
using NUnit.Framework;

namespace LudusTest;

public class GenerateMapUnitTest
{
    public MapInitConfig MapInitConfig { get; set; }
    public Map Map { get; set; }
    
    [SetUp]
    public void Setup()
    {
        var worldConfig = new WorldInitConfig
        {
            Dimensions = new(10, 10)
        };
        WorldManager.GenerateWorld(worldConfig);



    }

    [Test]
    [Order(1)]
    public void TestNewMap()
    {
        MapInitConfig = new MapInitConfig()
        {
            Size = new Vector2I(275, 275),
        };

        Map = MapGenerator.Generate(MapInitConfig);
        
        
        Assert.Pass("TestNewMap");
    }
    
    [Test]
    [Order(20)]
    public void Test1()
    {
        Assert.Pass("Test1");
    }
    
    [Test]
    [Order(10)]
    public void Test2()
    {
        Assert.Pass("Test2");
    }
}