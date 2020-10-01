export class Job {
  id: number;
  cron: string;
  status: string;
  lastFinishedDateTime: Date;
  name: string;
  typeName: string;
  type: number;
  databaseEnvironmentId: string;
  tagId: number;
  containerId:  string;
}
