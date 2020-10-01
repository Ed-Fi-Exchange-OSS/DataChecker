// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MSDF.DataChecker.Persistence.Tags;
using System;

namespace MSDF.DataChecker.Services.Models
{
    public class TagBO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IsUsed { get; set; }

        public TagBO() { }

        public TagBO(Tag entity) 
        {
            this.Created = entity.Created;
            this.Description = entity.Description;
            this.Id = entity.Id;
            this.IsPublic = entity.IsPublic;
            this.Name = entity.Name.ToUpper();
            this.Updated = entity.Updated;
        }
    }
}
