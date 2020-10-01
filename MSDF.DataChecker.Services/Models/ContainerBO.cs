// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using MSDF.DataChecker.Persistence.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MSDF.DataChecker.Services.Models
{
    public class ContainerBO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int ContainerTypeId { get; set; }
        public ContainerTypeBO ContainerType { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public Guid? ParentContainerId { get; set; }
        public List<ContainerBO> ChildContainers { get; set; }
        public List<RuleBO> Rules { get; set; }
        public bool IsDefault { get; set; }
        public CommunityUserBO CommunityUser { get; set; }
        public string Description { get; set; }
        public int EnvironmentType { get; set; }
        public List<TagBO> Tags { get; set; }
        public bool TagIsInherited { get; set; }
        public string ParentContainerName { get; set; }
        public int? RuleDetailsDestinationId { get; set; }
        public ContainerDestinationBO ContainerDestination { get; set; }
        public CatalogBO CatalogEnvironmentType { get; set; }
        public bool CreateNewCollection { get; set; }
    }

    public class ContainerTypeBO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CollectionCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<CollectionCategory> ChildContainers { get; set; }
        public List<RuleBO> Rules { get; set; }
        public int ValidRules { get; set; }
        public int LastStatus { get; set; }
        public int AmountRules { get; set; }
        public Boolean IsDefault { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public int ContainerTypeId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationDescription { get; set; }
        public string OrganizationName { get; set; }
        public string Description { get; set; }
        public int EnvironmentType { get; set; }
        public List<TagBO> Tags { get; set; }
        public int? RuleDetailsDestinationId { get; set; }

        public CollectionCategory()
        {

        }

        public CollectionCategory(Container container)
        {
            Id = container.Id;
            Name = container.Name;
            IsDefault = container.IsDefault;
            ContainerTypeId = container.ContainerTypeId;
            ///CreatedByUserId = container.CreatedByUserId;
            EnvironmentType = container.EnvironmentType;
            RuleDetailsDestinationId = container.RuleDetailsDestinationId;

            //if (container.CommunityUser != null)
            //{
            //    OrganizationId = container.CommunityUser.CommunityOrganizationId ?? (new Guid());
            //    OrganizationName = container.CommunityUser.Organization.NameOfInstitution;
            //}

            Description = container.Description;
            OrganizationDescription = "";
            //if (container.CommunityUser != null)
            //{
            //    Email = container.CommunityUser.Email;
            //    UserName = container.CommunityUser.Name;
            //}            

            ChildContainers = container.ChildContainers.Any() ? container.ChildContainers.Select(
                m => new CollectionCategory()
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    OrganizationDescription = "",
                    ContainerTypeId=m.ContainerTypeId,
                    //CreatedByUserId = m.CreatedByUserId
                }).ToList() : new List<CollectionCategory>();
        }
    }

    public class ContainerDestinationBO
    {
        public int Id { get; set; }
        public Guid ContainerId { get; set; }
        public int CatalogId { get; set; }
        public string DestinationName { get; set; }
        public string DestinationStructure { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid UserId { get; set; }
        public int Version { get; set; }
    }
}
