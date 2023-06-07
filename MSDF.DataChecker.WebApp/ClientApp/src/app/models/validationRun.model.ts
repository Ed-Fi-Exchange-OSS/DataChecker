export class ValidationRun {
  Id: number;
  DatabaseEnvironmentId: string;
  RunStatus: string;
  HostServer: string;
  HostDatabase: string;
  Source: string;
  StartTime: Date;
  EndTime?: Date;
}
