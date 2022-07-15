/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace Autodesk.Forge.Libs
{
    /// <summary>
    /// Location
    /// </summary>
    public class LocationNode
    {
        public LocationNode()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Children = new List<LocationNode>();
        }

        /// <summary>
        /// Node id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Node Category
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Node Type
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Node name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The list of child location
        /// </summary>
        public List<LocationNode> Children { get; set; }
    }
}
