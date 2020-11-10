import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { UtilService } from './util.service';

@Injectable()
export class RuleExecutionLogDetailService {

  private component = "RuleExecutionLogDetails";
  private url = UtilService.apiUrl() + this.component;

  constructor(private http: HttpClient) { }

  public getByRuleExecutionLogAsync(id: number): Observable<any> {
    return this.http
      .get(this.url + "/RuleExecutionLogAsync/" + id, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public getLastRuleExecutionLogByEnvironmentAndRuleAsync(environmentId: string, ruleId: string): Observable<any> {
    return this.http
      .get(this.url + "/LastRuleExecutionLogByEnvironmentAndRule/" + environmentId + "/" + ruleId, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public exportToCsvByRuleExecutionLogAsync(id: number): string {
    return this.url + "/ExportToCsvAsync/" + id;
  }

  public exportToTableByRuleExecutionLogAsync(id: number): Observable<any> {
    return this.http
      .get(this.url + "/ExportToTableAsync/" + id, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public executeDiagnosticSqlFromLogIdAsync(id: number): Observable<any> {
    return this.http
      .get(this.url + "/ExecuteDiagnosticSqlFromLog/" + id, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }
}
