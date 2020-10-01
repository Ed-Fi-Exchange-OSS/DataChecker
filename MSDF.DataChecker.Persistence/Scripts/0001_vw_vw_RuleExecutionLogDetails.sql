-- SPDX-License-Identifier: Apache-2.0
-- Licensed to the Ed-Fi Alliance under one or more agreements.
-- The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
-- See the LICENSE and NOTICES files in the project root for more information.

if object_id('destination.vw_RuleExecutionLogDetails','v') is not null
drop view destination.vw_RuleExecutionLogDetails;
go


Create view destination.vw_RuleExecutionLogDetails as
SELECT
	RuleExecutionLogs.Id as RuleExecutionLogsId,
	RuleExecutionLogs.RuleId,
	RuleExecutionLogs.DatabaseEnvironmentId,
	RuleExecutionLogs.Response,
	RuleExecutionLogs.statusId,
	RuleExecutionLogs.ExecutionDate,
	RuleExecutionLogs.ExecutionTimeMs,
	RuleExecutionLogs.ExecutedSql,
	RuleExecutionLogs.RuleDetailsDestinationId,
	RuleExecutionLogs.DetailsSchema,
	Rules.Name RuleName   ,
	Rules.Description RuleDescription    ,
	Rules.ErrorMessage    ,
	Rules.ErrorSeverityLevel     ,
	Rules.Resolution     ,
	Rules.DiagnosticSql    ,
	Rules.Version         ,
	Rules.RuleIdentification,
	ISNULL(Rules.MaxNumberResults, DatabaseEnvironments.MaxNumberResults) MaxNumberResults,
	DatabaseEnvironments.Name DatabaseEnvironmentName,
	Destinations.Name DestinationTableName,
	Containers.Name ContainerName,
	Collections.Name CollectionName,
	Environments.Name  EnvironmentsName,
	RANK() OVER (PARTITION BY Rules.id    ORDER BY RuleExecutionLogs.Id desc) rule_order
FROM
	destination.RuleExecutionLogs
	join datachecker.Rules
		on Rules.id = RuleExecutionLogs.RuleId
	join datachecker.DatabaseEnvironments
		on DatabaseEnvironments.id = RuleExecutionLogs.DatabaseEnvironmentId
	join datachecker.Containers
		on rules.ContainerId = Containers.Id
	join datachecker.Containers as collections
		on containers.ParentContainerId = collections.Id
	LEFT join core.Catalogs as Destinations
		on Destinations.Id = RuleExecutionLogs.RuleDetailsDestinationId
	Left join core.Catalogs as Environments
		on Environments.id = Collections.EnvironmentType;
