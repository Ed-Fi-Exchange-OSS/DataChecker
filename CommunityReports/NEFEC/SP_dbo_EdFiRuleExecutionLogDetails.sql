USE [DataCheckerPersistence]
GO

/****** Object:  StoredProcedure [dbo].[EdFiRuleExecutionLogDetails]    Script Date: 7/6/2021 3:45:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[EdFiRuleExecutionLogDetails]
AS
DECLARE @JsonKeys TABLE (
	[Key] VARCHAR(128)
);

INSERT INTO @JsonKeys
SELECT Distinct Keys.[key]
FROM DataCheckerPersistence.destination.EdFiRuleExecutionLogDetails t
CROSS APPLY (
	SELECT [key] [key]
	FROM OPENJSON((SELECT t.OtherDetails))
) Keys
ORDER BY Keys.[Key];

--SELECT * FROM @JsonKeys;

/* Declare cursor to loop through @JsonKeys table. */
DECLARE KeyCursor CURSOR FOR SELECT [Key] FROM @JsonKeys;

DECLARE @query VARCHAR(MAX);
DECLARE @Key VARCHAR(MAX);
SET @query = 'SELECT ';

/* Start cursor and grab the first value. */
OPEN KeyCursor 
FETCH NEXT FROM KeyCursor INTO @Key;

WHILE @@FETCH_STATUS = 0
BEGIN
	SET @query = @query + 'JSON_VALUE(OtherDetails,''$.' + @Key + ''') AS ''' + @Key + ''', ';
	FETCH NEXT FROM KeyCursor INTO @Key;
END
CLOSE KeyCursor;
DEALLOCATE KeyCursor;

--SET @query = @query + 'RuleExecutionLogId FROM DataCheckerPersistence.destination.EdFiRuleExecutionLogDetails';
SET @query = @query + 'EducationOrganizationId,StudentUniqueId,CourseCode,Discriminator,ProgramName,StaffUniqueId,res.RuleId, ereld.RuleExecutionLogId AS RuleExecutionLogId FROM DataCheckerPersistence.destination.RuleExecutionLogs res INNER JOIN DataCheckerPersistence.destination.EdFiRuleExecutionLogDetails ereld on ereld.ruleExecutionLogId = res.Id';
SET @query = @query + ' INNER JOIN [DataCheckerPersistence].[bi].[vw_RuleExecutionLogDetails] vreld on vreld.RuleExecutionLogsId=ereld.RuleExecutionLogId where vreld.rule_order = 1'

PRINT @query;
EXEC(@query);
--GO;
GO


