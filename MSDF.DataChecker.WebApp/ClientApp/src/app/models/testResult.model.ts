export class TestResult {
  id: number;
  evaluation: boolean;
  executionTimeMs: number;
  lastExecuted: Date;
  result: number;
  hasError: boolean;
  status: number;

  exceptionMessage: string;
  // This variable are only for front-end purposes
  displayErrorDetails: boolean;
}
