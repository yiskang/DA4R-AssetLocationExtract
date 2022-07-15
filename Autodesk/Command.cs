// (C) Copyright 2022 by Autodesk, Inc. 
//
// Permission to use, copy, modify, and distribute this software
// in object code form for any purpose and without fee is hereby
// granted, provided that the above copyright notice appears in
// all copies and that both that copyright notice and the limited
// warranty and restricted rights notice below appear in all
// supporting documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK,
// INC. DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL
// BE UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is
// subject to restrictions set forth in FAR 52.227-19 (Commercial
// Computer Software - Restricted Rights) and DFAR 252.227-7013(c)
// (1)(ii)(Rights in Technical Data and Computer Software), as
// applicable.
//

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Forge.Libs;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using DesignAutomationFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Autodesk.Revit.DB.Mechanical;

namespace Autodesk.Forge
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalDBApplication
    {
        public ExternalDBApplicationResult OnStartup(ControlledApplication application)
        {
            DesignAutomationBridge.DesignAutomationReadyEvent += HandleDesignAutomationReadyEvent;
            return ExternalDBApplicationResult.Succeeded;
        }

        public ExternalDBApplicationResult OnShutdown(ControlledApplication application)
        {
            return ExternalDBApplicationResult.Succeeded;
        }

        private void HandleDesignAutomationReadyEvent(object sender, DesignAutomationReadyEventArgs e)
        {
            LogTrace("Design Automation Ready event triggered...");

            e.Succeeded = true;
            e.Succeeded = this.DoTask(e.DesignAutomationData);
        }
        private bool DoTask(DesignAutomationData data)
        {
            if (data == null)
                return false;

            Application app = data.RevitApp;
            if (app == null)
            {
                LogTrace("Error occured");
                LogTrace("Invalid Revit App");
                return false;
            }

            string modelPath = data.FilePath;
            if (string.IsNullOrWhiteSpace(modelPath))
            {
                LogTrace("Error occured");
                LogTrace("Invalid File Path");
                return false;
            }

            var doc = data.RevitDoc;
            if (doc == null)
            {
                LogTrace("Error occured");
                LogTrace("Invalid Revit DB Document");
                return false;
            }

            try
            {
                LogTrace("Collecting Rooms...");

                var locationNodes = new List<LocationNode>();

                FilteredElementCollector collector = new FilteredElementCollector(doc)
                                                        .WhereElementIsNotElementType()
                                                        .OfClass(typeof(SpatialElement));
               
                var rooms = collector.Where(e => e.GetType() == typeof(Room)).Cast<Room>().OrderBy(room => room.Level?.Elevation);

                LogTrace($"... {rooms.Count()} rooms found in the host...");

                foreach (Room room in rooms)
                {
                    var roomNode = new LocationNode()
                    {
                        Id = room.UniqueId,
                        Name = room.Name,
                        Category = "Spaces",
                        Type = "Room"
                    };

                    LogTrace($"Finding assets in room `{room.Name}`...");

                    var intersection = this.FindIntersetedElementsByRoom(room, doc);
                    LogTrace($"... {intersection.Count} assets found...");

                    var assetNodes = intersection.Select(asset => new LocationNode()
                    {
                        Id = asset.UniqueId,
                        Name = asset.Name,
                        Category = "Assets",
                        Type = asset.Category?.Name ?? "Unknown"
                    });

                    roomNode.Children.AddRange(assetNodes);
                    locationNodes.Add(roomNode);
                }

                LogTrace($"Producing JSON string result...");
                var result = JsonConvert.SerializeObject(
                    locationNodes,
                    Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });

                LogTrace($"Writting JSON result to `result.json`...");
                using (StreamWriter sw = File.CreateText("result.json"))
                {
                    sw.WriteLine(result);
                    sw.Close();
                }

                LogTrace($"... DONE ...");
            }
            catch (Autodesk.Revit.Exceptions.InvalidPathArgumentException ex)
            {
                this.PrintError(ex);
                return false;
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException ex)
            {
                this.PrintError(ex);
                return false;
            }
            catch (Autodesk.Revit.Exceptions.InvalidOperationException ex)
            {
                this.PrintError(ex);
                return false;
            }
            catch (Exception ex)
            {
                this.PrintError(ex);
                return false;
            }

            LogTrace("Successfully extract asset locations` to `result.json`...");

            return true;
        }

        List<Element> FindIntersetedElementsByRoom(Room room, Document doc)
        {
            if (room == null) return null;

            // Find elements in rooms
            GeometryElement geomElement = room.ClosedShell;//.get_Geometry(new Options());
            Solid solid = null;
            foreach (GeometryObject geomObj in geomElement)
            {
                solid = geomObj as Solid;
                if (solid != null) break;
            }

            if (solid == null) return null;

            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();

            builtInCats.Add(BuiltInCategory.OST_CableTray);
            builtInCats.Add(BuiltInCategory.OST_CableTrayFitting);
            builtInCats.Add(BuiltInCategory.OST_Conduit);
            builtInCats.Add(BuiltInCategory.OST_ConduitRun);
            builtInCats.Add(BuiltInCategory.OST_ConduitFitting);
            builtInCats.Add(BuiltInCategory.OST_CommunicationDevices);
            builtInCats.Add(BuiltInCategory.OST_DuctTerminal);
            builtInCats.Add(BuiltInCategory.OST_DuctCurves);
            builtInCats.Add(BuiltInCategory.OST_DuctFitting);
            builtInCats.Add(BuiltInCategory.OST_DuctAccessory);
            builtInCats.Add(BuiltInCategory.OST_DataDevices);
            builtInCats.Add(BuiltInCategory.OST_ElectricalEquipment);
            builtInCats.Add(BuiltInCategory.OST_ElectricalFixtures);
            builtInCats.Add(BuiltInCategory.OST_FireAlarmDevices);
            builtInCats.Add(BuiltInCategory.OST_LightingDevices);
            builtInCats.Add(BuiltInCategory.OST_LightingFixtures);
            builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);
            builtInCats.Add(BuiltInCategory.OST_NurseCallDevices);
            builtInCats.Add(BuiltInCategory.OST_PipeCurves);
            builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
            builtInCats.Add(BuiltInCategory.OST_PipeFitting);
            builtInCats.Add(BuiltInCategory.OST_PlumbingFixtures);
            builtInCats.Add(BuiltInCategory.OST_SecurityDevices);
            builtInCats.Add(BuiltInCategory.OST_Sprinklers);
            builtInCats.Add(BuiltInCategory.OST_TelephoneDevices);


            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            //collector.OfClass(typeof (FamilyInstance));
            collector.WhereElementIsNotElementType();
            collector.WherePasses(filter1);
            collector.WherePasses(new ElementIntersectsSolidFilter(solid));

            return collector.ToList();
        }

        private void PrintError(Exception ex)
        {
            LogTrace("Error occured");
            LogTrace(ex.Message);

            if (ex.InnerException != null)
                LogTrace(ex.InnerException.Message);
        }

        /// <summary>
        /// This will appear on the Design Automation output
        /// </summary>
        public static void LogTrace(string format, params object[] args)
        {
#if DEBUG
            System.Diagnostics.Trace.WriteLine(string.Format(format, args));
#endif
            System.Console.WriteLine(format, args);
        }
    }
}
