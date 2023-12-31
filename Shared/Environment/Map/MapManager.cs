﻿using BaseManager = Bitspoke.GodotEngine.Managers.Manager;
using Bitspoke.Ludus.Shared.Environment.Map.Generation;
using Godot;

namespace Bitspoke.Ludus.Shared.Environment.Map;

public partial class MapManager : BaseManager
{
    #region Properties
        
    private static MapManager instance { get; set; } = null;
    public static MapManager Instance
    {
        get
        {
            if (instance == null)
                instance = new MapManager();

            return instance;
        }
    }
    
    public override string NodeName => GetType().Name;
    public override Node Node => this;
        
    #endregion

    #region Constructors and Initialisation
        
    #endregion

    #region Overrides

    public override void Init() {}
    public override void AddComponents() {}
    public override void ConnectSignals() {}

    #endregion
    
    #region Methods

    public static Map GenerateMap(MapInitConfig initConfig)
    {
        return MapGenerator.Generate(initConfig);
    }
        
    #endregion


        
}