// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;

namespace MSDF.DataChecker.Services.Models
{
    public class TableResult
    {
        public int Columns { get; set; }
        public List<string> ColumnsName { get; set; }
        public int Rows { get; set; }
        public List<Dictionary<string,object>> Information { get; set; }
        public string MessageError { get; set; }

        public TableResult()
        {
            ColumnsName = new List<string>();
            Information = new List<Dictionary<string, object>>();
        }
    }
}
