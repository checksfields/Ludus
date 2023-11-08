using System.Text;
using Bitspoke.Core.Signal;
using Bitspoke.Core.Utils.String;
using Bitspoke.GodotEngine.Components.Console;
using Bitspoke.Ludus.Shared.Common.Entities;
using Godot;

namespace Bitspoke.Ludus.Shared.Common.Controllers.Admin
{
    public class AdminCommandController : ICommandController
    {
        public IConsole OutputConsole { get; set; }
        
        public AdminCommandController()
        {
            
        }
        
        public AdminCommandController(IConsole outputConsole)
        {
            OutputConsole = outputConsole;
        }
        
        public void ProcessCommand(string command)
        {
            string[] commandArgs = StringExtensions.Split(command.ToLower(), " ", false);

            if (commandArgs.Length <= 0)
            {
                Log.Error("Please enter at least 1 command argument");
                return;
            }
            
            switch (commandArgs[0])
            {
                case "help":
                    Help();
                    break;
                case "q":
                case "quit":
                case "exit":
                    SignalManager.Emit(AdminCommandControllerSignals.QUIT_REQUEST);
                    break;
                case "show":
                    SetVisible(true, commandArgs);
                    break;
                case "hide":
                    SetVisible(false, commandArgs);
                    break;
                case "debug":
                    Debug(commandArgs);
                    break;
                case "report":
                    Report(commandArgs);
                    break;
                default:
                    return;
            }
            
            Log.Debug($"Command: {command}");
        }

        private void Debug(string[] commandArgs)
        {
            if (commandArgs.Length < 2)
            {
                var message = $"'debug' command must have at lease 2 elements.  'debug' command supplied: {commandArgs.ToStringExt()}";
                Log.Error(message);
                WriteToConsole(message);
                return;
            }

            switch (commandArgs[1])
            {
                case "map":
                    DebugMap(commandArgs);
                    break;
                case "layer":
                    DebugLayer(commandArgs);
                    break;
                case "region":
                    throw new NotImplementedException();
                    break;
                case "cell":
                    throw new NotImplementedException();
                    break;
                default:
                    break;
            }
        }

        private void DebugMap(string[] commandArgs)
        {
            if (commandArgs.Length < 3)
            {
                var message = $"'debug layer' command must have at lease 3 elements.  'debug layer' command supplied: {commandArgs.ToStringExt()}";
                Log.Error(message);
                WriteToConsole(message);
                return;
            }

            switch (commandArgs[2])
            {
                case "plants":
                    DebugMapPlants(commandArgs);
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// debug map plants all   -s
        ///                  grass -h
        /// </summary>
        /// <param name="visible"></param>
        /// <param name="commandArgs"></param>
        private void DebugMapPlants(string[] commandArgs)
        {
            if (commandArgs.Length < 5)
            {
                var message = $"'debug map plants [all/plantDefKey] [action]' command must have at lease 5 elements.  'debug map plants' command supplied: {commandArgs.ToStringExt()}";
                Log.Error(message);
                WriteToConsole(message);
                return;
            }

            var on = commandArgs[3];
            if (on == "all")
                on = null;
            
            switch (commandArgs[4])
            {
                case "-s":
                case "-show":
                    SignalManager.Emit(AdminCommandControllerSignals.SET_PLANTS_VISIBLE, new object[] { true, on });
                    Log.Info($"debug layer terrain {on ?? "all"} -show");
                    break;
                case "-h":
                case "-hide":
                    SignalManager.Emit(AdminCommandControllerSignals.SET_PLANTS_VISIBLE, new object[] { false, on });
                    Log.Info($"debug layer terrain {on ?? "all"} -hide");
                    break;
                default:
                    break;
            }
        }
        
        private void DebugLayer(string[] commandArgs)
        {
            if (commandArgs.Length < 3)
            {
                var message = $"'debug layer' command must have at lease 3 elements.  'debug layer' command supplied: {commandArgs.ToStringExt()}";
                Log.Error(message);
                WriteToConsole(message);
                return;
            }

            switch (commandArgs[2])
            {
                case "terrain":
                    DebugLayerTerrain(commandArgs);
                    break;
                case "fertility":
                    //DebugLayerFertility(commandArgs);
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// debug layer terrain all      -s
        ///                     soilrich -h
        /// </summary>
        /// <param name="visible"></param>
        /// <param name="commandArgs"></param>
        private void DebugLayerTerrain(string[] commandArgs)
        {
            if (commandArgs.Length < 5)
            {
                var message = $"'debug layer terrain [all/terrainDefKey] [action]' command must have at lease 5 elements.  'debug layer terrain' command supplied: {commandArgs.ToStringExt()}";
                Log.Error(message);
                WriteToConsole(message);
                return;
            }

            var on = commandArgs[3];
            if (on == "all")
                on = null;
            
            switch (commandArgs[4])
            {
                case "-s":
                case "-show":
                    SignalManager.Emit(AdminCommandControllerSignals.SET_TERRAIN_LAYER_VISIBLE, new object[] { true, on });
                    Log.Info($"debug layer terrain {on ?? "all"} -show");
                    break;
                case "-h":
                case "-hide":
                    SignalManager.Emit(AdminCommandControllerSignals.SET_TERRAIN_LAYER_VISIBLE, new object[] { false, on });
                    Log.Info($"debug layer terrain {on ?? "all"} -hide");
                    break;
                default:
                    break;
            }
        }
        
        private void Report(string[] commandArgs)
        {
            if (commandArgs.Length < 2)
            {
                var message = $"'report' command must have at lease 2 elements.  'report' command supplied: {commandArgs.ToStringExt()}";
                Log.Error(message);
                WriteToConsole(message);
                return;
            }

            switch (commandArgs[1])
            {
                case "plants":
                    ReportPlants(commandArgs);
                    break;
                default:
                    break;
            }
        }
        
        private void ReportPlants(string[] commandArgs)
        {
            if (commandArgs.Length < 3)
            {
                var message = $"'report plants' command must have at lease 3 elements.  'report plants' command supplied: {commandArgs.ToStringExt()}";
                Log.Error(message);
                WriteToConsole(message);
                return;
            }

            switch (commandArgs[2])
            {
                case "-full":
                    var map = Find.CurrentMap;
                    if (map == null)
                    {
                        var nullMapMessage = "Cannot run report on plants, no map is loaded.";
                        Log.Error(nullMapMessage);
                        WriteToConsole(nullMapMessage);
                        break;
                    }
                    
                    
                    var message = new StringBuilder();
                    var plants = map.Entities.Get(EntityType.Plant);
                    message.AppendLine($"Total: {plants.Count}");
                    message.AppendLine($"Grass: {plants.Count(c => c.Def.Key == "grass").ToString() ?? "NaN"}");
                    
                    
                    Log.Info(message.ToString());
                    WriteToConsole(message.ToString());
                    break;
                default:
                    break;
            }
        }
        
        private void SetVisible(bool visible, string[] commandArgs)
        {
            if (commandArgs.Length < 2)
            {
                var message = $"'show' command must have at lease 2 elements.  'show' command supplied: {commandArgs.ToStringExt()}";
                Log.Error(message);
                WriteToConsole(message);
                return;
            }

            switch (commandArgs[1])
            {
                case "terrain":
                    SetVisibleTerrain(visible, commandArgs);
                    break;
                case "plant":
                    SetVisiblePlants(visible, commandArgs);
                    break;
                default:
                    break;
            }
        }
        

        private void SetVisiblePlants(bool visible, string[] commandArgs)
        {
            if (commandArgs.Length < 3)
            {
                var message = $"'show plants' command must have at lease 3 elements.  'show' command supplied: {commandArgs.ToStringExt()}";
                Log.Error(message);
                WriteToConsole(message);
                return;
            }

            switch (commandArgs[2])
            {
                case "all":
                    if (commandArgs.Length > 3)
                    {
                        if (commandArgs[3] == "-h")
                            visible = false;
                        if (commandArgs[3] == "-s")
                            visible = true;
                    }
                    SignalManager.Emit(AdminCommandControllerSignals.SET_PLANTS_VISIBLE, new object[] { visible, null });
                    break;
                case "grass":
                    
                    break;
                default:
                    break;
            }
        }
        
        
        private void SetVisibleTerrain(bool visible, string[] commandArgs)
        {
            if (commandArgs.Length < 3)
            {
                var message = $"'show terrain' command must have at lease 3 elements.  'show' command supplied: {commandArgs.ToStringExt()}";
                Log.Error(message);
                WriteToConsole(message);
                return;
            }

            switch (commandArgs[2])
            {
                case "all":
                    SignalManager.Emit(AdminCommandControllerSignals.SET_TERRAIN_LAYER_VISIBLE, new object[] { visible, null } );
                    break;
                case "soilrich":
                    
                    break;
                default:
                    break;
            }
        }

        private void Help()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Help Menu:");
            sb.AppendLine("TODO");
            
            WriteToConsole(sb.ToString());
        }
       
        public void WriteToConsole(string message)
        {
            if (OutputConsole != null)
                OutputConsole.AppendTextToOutput(message, true);
        }
    }
}
