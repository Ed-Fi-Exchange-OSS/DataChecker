import { UserParam } from "./userParam.model";

export class DatabaseEnvironment {
  id: string;
  name: string;
  database: string;
  dataSource: string
  user: string;
  password: string;
  extradata: string;
  version: number;
  securityIntegrated: boolean;

  connectionString: string;
  tablesData: object;
  createdByUserId: string;
  createdDate: string;

  userParams: UserParam[];

  maxNumberResults: number;

  timeoutInMinutes: number;
}
