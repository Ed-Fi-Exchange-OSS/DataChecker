// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

//using MSDF.DataChecker.Persistence.CommunityUser;
//using MSDF.DataChecker.Persistence.CommunityUsers;
//using MSDF.DataChecker.Persistence.ComunityOrganization;
//using MSDF.DataChecker.Services.Models;
//using System;
//using System.Threading.Tasks;

//namespace MSDF.DataChecker.Services
//{
//    public interface ICommunityUserService
//    {
//        Task UpdateAsync(CommunityUserBO model);
//        Task<CommunityUserBO> GetAsync(Guid id);
//    }
//    public class CommunityUserService : ICommunityUserService
//    {
//        ICommunityUserQueries _communityUserQueries;
//        ICommunityUserCommands _communityUserCommands;

//        public CommunityUserService(
//            ICommunityUserQueries communityUserQueries , 
//            ICommunityUserCommands communityUserCommands)
//        {
//            _communityUserQueries = communityUserQueries;
//            _communityUserCommands = communityUserCommands;
//        }

//        public async Task UpdateAsync(CommunityUserBO model)
//        {
//            await _communityUserCommands.UpdateAsync(MapModelToEntity(model));
//        }

//        public async Task<CommunityUserBO> GetAsync(Guid id)
//        {
//            var entity = await _communityUserQueries.GetAsync(id);
//            if (entity != null)
//                return MapEntityToModel(entity);

//            return null;
//        }

//        private CommunityUserBO MapEntityToModel(CommunityUserModel model)
//        {
//            CommunityUserBO entity = new CommunityUserBO
//            {
//                CommunityOrganizationId = model.CommunityOrganizationId,
//                CreateByUserId = model.CreateByUserId,
//                CreatedDate = model.CreatedDate,
//                Email = model.Email,
//                Id = model.Id,
//                LastUpdatedDate = model.LastUpdatedDate,
//                LastUpdatedUserId = model.LastUpdatedUserId,
//                Name = model.Name
//            };

//            if (model.Organization != null)
//                entity.Organization = new CommunityOrganizationBO { 
//                    NameOfInstitution=model.Organization.NameOfInstitution,
//                    PrimaryContactEmail = model.Organization.PrimaryContactEmail,
//                    PrimaryContactName = model.Organization.PrimaryContactName,
//                    PrimaryContactPhone = model.Organization.PrimaryContactPhone
//                };

//            return entity;
//        }

//        private CommunityUserModel MapModelToEntity(CommunityUserBO entity)
//        {
//            CommunityUserModel model = new CommunityUserModel
//            {
//                CommunityOrganizationId = entity.CommunityOrganizationId,
//                CreateByUserId = entity.CreateByUserId,
//                CreatedDate = entity.CreatedDate,
//                Email = entity.Email,
//                Id = entity.Id,
//                LastUpdatedDate = entity.LastUpdatedDate,
//                LastUpdatedUserId = entity.LastUpdatedUserId,
//                Name = entity.Name
//            };

//            if (entity.Organization != null)
//                model.Organization = new CommunityOrganizationModel
//                {
//                    Id = entity.Id,
//                    NameOfInstitution = entity.Organization.NameOfInstitution,
//                    PrimaryContactEmail = entity.Organization.PrimaryContactEmail,
//                    PrimaryContactName = entity.Organization.PrimaryContactName,
//                    PrimaryContactPhone = entity.Organization.PrimaryContactPhone
//                };

//            return model;
//        }
//    }
//}
