// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;

namespace MSDF.DataChecker.Services.Models
{
    public class CommunityUserBO
    {
        public CommunityUserBO()
        {
            this.CreatedDate = DateTime.UtcNow;
            this.LastUpdatedDate = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public Guid CreateByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? LastUpdatedUserId { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public Guid? CommunityOrganizationId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public CommunityOrganizationBO Organization { get; set; }
    }

    public class CommunityOrganizationBO
    { 
        public string NameOfInstitution { get; set; }
        public string PrimaryContactName { get; set; }
        public string PrimaryContactPhone { get; set; }
        public string PrimaryContactEmail { get; set; }
    }
}
