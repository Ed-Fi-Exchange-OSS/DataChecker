import { TestResult } from "./testResult.model";
import { Tag } from "./tag.model";

export class Rule {
  id: string;
  name: string;
  category: string;
  description: string;
  errorSeverityLevel: number;
  resolution: string;
  diagnosticSql: string;
  edFiODSCompatibilityVersion: string;
  ruleIdentification: string;
  version: string;
  previousVersions: Rule[];
  enabled: boolean;
  results: TestResult[];

  createdByUserId: string;
  containerId: string;

  // This variables are only for front-end purposes
  isExecuting: boolean;
  testResults: TestResult[];
  displayResults: boolean;
  displayDetails: boolean;
  displayError: boolean;
  counter: number;
  lastExecution: Date;
  lastStatus: number;
  status: number;

  environmentTypeText: string;
  tags: Tag[];

  collectionRuleDetailsDestinationId: number;
  selected: boolean;
  collectionContainerName: string;

  maxNumberResults: number;
  idLastRuleExecutionLog: number;

  collectionName: string;
  containerName: string;

  dateUpdated?: Date;
  strDateUpdated?: string;
}

export class RuleFilter {
  rulesSuccess: boolean;
  rulesValidation: boolean;
  rulesError: boolean;
  rulesNotRan: boolean;
}
