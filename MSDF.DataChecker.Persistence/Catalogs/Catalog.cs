// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSDF.DataChecker.Persistence.Catalogs
{
    [Table("Catalogs", Schema = "core")]
    public class Catalog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(350)]
        public string CatalogType { get; set; }
        [MaxLength(350)]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Updated { get; set; }
    }
}
