// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MSDF.DataChecker.Persistence.Collections;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSDF.DataChecker.Persistence.Tags
{
    [Table("Tags", Schema = "datachecker")]
    public class Tag
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

    [Table("TagEntities", Schema = "datachecker")]
    public class TagEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TagId { get; set; }
        [ForeignKey("TagId")]
        public Tag Tag { get; set; }
        public Guid? ContainerId { get; set; }
        [ForeignKey("ContainerId")]
        public virtual Container Container { get; set; }
        public Guid? RuleId { get; set; }
        [ForeignKey("RuleId")]
        public virtual Rules.Rule Rule { get; set; }
    }
}
