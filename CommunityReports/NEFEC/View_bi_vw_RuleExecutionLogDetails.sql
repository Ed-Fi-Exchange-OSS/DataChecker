USE [DataCheckerPersistence]
GO

/****** Object:  View [bi].[vw_RuleExecutionLogDetails]    Script Date: 7/6/2021 3:43:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE view [bi].[vw_RuleExecutionLogDetails] as
SELECT
	RuleExecutionLogs.Id as RuleExecutionLogsId,
	RuleExecutionLogs.RuleId,
	RuleExecutionLogs.DatabaseEnvironmentId,
	RuleExecutionLogs.Response,
	RuleExecutionLogs.Result,
	RuleExecutionLogs.Evaluation,
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

GO


