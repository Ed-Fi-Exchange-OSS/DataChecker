// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Collections.Generic;

namespace MSDF.DataChecker.Services.Models
{
    public class SearchTagBO
    {
        public List<TagBO> TagsSelected { get; set; }
        public List<ContainerBO> Collections { get; set; }
        public List<ContainerBO> Containers { get; set; }
        public List<RuleBO> Rules { get; set; }

        public SearchTagBO()
        {
            this.TagsSelected = new List<TagBO>();
            this.Collections = new List<ContainerBO>();
            this.Containers = new List<ContainerBO>();
            this.Rules = new List<RuleBO>();
        }
    }
}
