// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSDF.DataChecker.Persistence.Base
{
    public abstract class BaseAuditModel
    {
        public BaseAuditModel()
        {
            CreatedDate = DateTime.UtcNow;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid CreateByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? LastUpdatedUserId { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public bool Deleted { get; set; }
    }
}
