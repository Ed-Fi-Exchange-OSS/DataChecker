export class TestResult {
  id: number;
  evaluation: boolean;
  isExecuting: boolean;
  executionTimeMs: number;
  lastExecuted: Date;
  result: number;
  hasError: boolean;
  status: number;
  ruleDetailsDestinationId: number;
  exceptionMessage: string;
  errorMessage: string;
  // This variable are only for front-end purposes
  displayErrorDetails: boolean;
}
