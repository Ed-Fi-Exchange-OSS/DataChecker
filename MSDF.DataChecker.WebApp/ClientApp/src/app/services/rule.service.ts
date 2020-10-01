import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { UtilService } from './util.service';
import { TestResult } from "../models/testResult.model";

@Injectable()
export class RuleService {
  private component = "rules";
  private url = UtilService.apiUrl() + this.component;
  private userId = UtilService.currentUserId();

  constructor(private http: HttpClient) { }

  public getRuleDetail(id: string, databaseEnvironmentId: string): Observable<TestResult[]> {
    let params = new HttpParams();
    return this.http
      .get(this.url + "/Results/" + id + "/" + databaseEnvironmentId, { headers: new HttpHeaders(), params: params })
      .pipe(map((result: TestResult[]) => result));
  }

  public addRule(rule: any): Observable<any> {
    rule.createdByUserId = this.userId;
    return this.http
      .post(this.url, rule)
      .pipe(map((result: any) => result));
  }

  public modifyRule(rule: any): Observable<any> {
    return this.http
      .post(this.url + "/Update", rule)
      .pipe(map((result: any) => result));
  }

  public deleteRule(ruleId: string): Observable<any> {
    return this.http
      .get(this.url + "/Delete/" + ruleId, {})
      .pipe(map((result: any) => result));
  }

  public executeRule(parameters: any): Observable<any> {
    return this.http
      .post(this.url + '/Run', parameters, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public executeDiagnostic(idDiagnostic: string, databaseEnvironmentId: string): Observable<any> {
    return this.http
      .post(this.url + '/RunDiagnosticAndReturnTable', { ruleExecutionLogId: idDiagnostic, databaseEnvironmentId: databaseEnvironmentId }, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public executeRuleTest(rule: any, databaseEnvironmentId: string): Observable<any> {
    return this.http
      .post(this.url + '/TestRun', { databaseEnvironmentId: databaseEnvironmentId, ruleToTest: rule }, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public searchRules(model: any): Observable<any> {
    return this.http
      .post(this.url + "/SearchRules", model)
      .pipe(map((result: any) => result));
  }

  public copyRuleTo(model: any): Observable<any> {
    return this.http
      .post(this.url + "/CopyRuleTo", model)
      .pipe(map((result: any) => result));
  }

  public deleteRules(model: any): Observable<any> {
    return this.http
      .post(this.url + "/DeleteByIds", model)
      .pipe(map((result: any) => result));
  }

  public assignTagsToRules(model: any): Observable<any> {
    return this.http
      .post(this.url + "/AssignTagsByIds", model)
      .pipe(map((result: any) => result));
  }

  public copyRulesToContainers(model: any): Observable<any> {
    return this.http
      .post(this.url + "/CopyToByIds", model)
      .pipe(map((result: any) => result));
  }

  public moveRuleToContainer(rules:any, container:any ): Observable<any> {
    return this.http
      .post(this.url + "/MoveRulesToContainer", { rules: rules, containerTo: container })
      .pipe(map((result: any) => result));
  }
}
