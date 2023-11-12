using System.Text;
using Bitspoke.Core.Common.Vector;
using Bitspoke.GodotEngine.Utils.Files;
using Bitspoke.Ludus.Shared.Entities.Containers.Extensions;
using Bitspoke.Ludus.Shared.Environment.Map;

namespace Bitspoke.Ludus.Shared.Tests.Maps;

public static class MapDebugExtension
{

    public static void DebugPrintAscii(this Map map)
    {
        
        var width = map.Width;
        var height = map.Height;
        
        var terrainDefs = map.Cells.TerrainDefs;
        var roofDefs = map.Cells.RoofDefs;
        var cells = map.Cells.Ordered;
        
        var terrainRows = new StringBuilder();
        var structureRows = new StringBuilder();
        var roofRows = new StringBuilder();
        var plantRows = new StringBuilder();
        
        for (int y = 0; y < height; y++)
        {
            var terrainRow = new StringBuilder();
            var structureRow = new StringBuilder();
            var roofRow = new StringBuilder();
            var plantRow = new StringBuilder();
         
            for (int x = 0; x < width; x++)
            {
                var index = new Vec2Int(x, y).ToIndex(width);

                var cell = cells[index];

                // ROOF
                var roofDef = cell.RoofDef;
                roofRow.Append(roofDef != null ? 'X' : " ").Append(' ');

                // STRUCTURE && TERRAIN
                var structureDef = cell.StructureDef; 
                if (structureDef == null && roofDef == null)
                    terrainRow.Append(cell.TerrainDef?.Ascii).Append(' ');
                else
                    if (roofDef == null)
                        terrainRow.Append(Convert.ToChar(62)).Append(' ');
                    else
                        terrainRow.Append("X").Append(' ');

                structureRow.Append(structureDef?.Ascii != null ? cell.StructureDef.Ascii : " ").Append(' ');
                

                // PLANT
                var plantDefs = cell.Entities?.PlantDefs();
                if (plantDefs != null && plantDefs.Count > 0)
                    plantRow.Append(plantDefs[plantDefs.Count-1]?.Ascii != null ? plantDefs[plantDefs.Count-1].Ascii : "?").Append(' ');
                else
                    plantRow.Append('.').Append(' ');
            }
            
            terrainRows.AppendLine(terrainRow.ToString());
            roofRows.AppendLine(roofRow.ToString());
            structureRows.AppendLine(structureRow.ToString());
            plantRows.AppendLine(plantRow.ToString());
        }

        GodotFileUtils.WriteToFile($"{GodotGlobal.SAVE_ROOT_PATH}/terrain.map", terrainRows.ToString());
        GodotFileUtils.WriteToFile($"{GodotGlobal.SAVE_ROOT_PATH}/structure.map", structureRows.ToString());
        GodotFileUtils.WriteToFile($"{GodotGlobal.SAVE_ROOT_PATH}/roof.map", roofRows.ToString());
        GodotFileUtils.WriteToFile($"{GodotGlobal.SAVE_ROOT_PATH}/plants.map", plantRows.ToString());
        
    }
    
}