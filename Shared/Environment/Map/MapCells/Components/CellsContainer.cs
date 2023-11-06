using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Bitspoke.Core.Common.Collections.Arrays;
using Bitspoke.Core.Common.Collections.Dictionaries;
using Bitspoke.Core.Utils.Primatives.Int;
using Bitspoke.GodotEngine.Utils.Vector;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.MapCells.Components;

[ImmutableObject(true)]
public class CellsContainer
{
    #region Properties
    // none
    public Map Map { get; set; }
    
    public BitspokeArray<MapCell?> Cells { get; set; }
    [JsonIgnore] public BitspokeArray<int> CellRegionMap { get; set; }
    
    public MapCell? this[int index] => Cells?[index] ?? null;
    
    #endregion

    #region Constructors and Initialisation

    public CellsContainer(int mapID) : this(Find.Map(mapID)) { }
    
    public CellsContainer(Map map)
    {
        Map = map;
        Init();
    }

    protected void Init()
    {
        Cells = new BitspokeArray<MapCell?>(Map.Area);
        CellRegionMap = new BitspokeArray<int>(Map.Area);
        InitialiseCells();
    }

    private void InitialiseCells()
    {
        Profiler.Start();
        var mapID = Map.ID;
        
        for (var index = 0; index < Cells.Length; index++)
        {
            var cell = new MapCell(index, mapID);
            Cells[index] = cell;
            CellRegionMap[index] = cell.RegionIndex;
        }

        Profiler.End();
    }

    #endregion

    #region Methods
    // none
    #endregion
    
}