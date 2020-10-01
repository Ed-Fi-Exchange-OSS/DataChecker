import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { UtilService } from './util.service';
import { UserParam } from '../models/userParam.model';

@Injectable()
export class UserParamService {
  private component = 'UserParam';
  private url = UtilService.apiUrl() + this.component;
  private userId = UtilService.currentUserId();

  constructor(private http: HttpClient) { }

  public getDatabaseInfo(databaseEnvironmentId: string): Observable<any> {
    return this.http
      .get(this.url + "/GetDatabaseInfo/" + databaseEnvironmentId, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public addUserParam(userParam: any): Observable<any> {
    userParam.createdByUserId = this.userId;
    return this.http
      .post(this.url, userParam)
      .pipe(map((result: any) => result));
  }

  public getUserParams(databaseEnvironmentId: string): Observable<any> {
    return this.http
      .get(this.url + "/" + databaseEnvironmentId, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public updateUserParam(userParam: UserParam): Observable<any> {
    return this.http
      .post(this.url + "/Update", userParam)
      .pipe(map((result: any) => result));
  }

  public updateUserParams(userParams: UserParam[], databaseEnvironmentId: string): Observable<any> {
    return this.http
      .post(this.url + "/UpdateUserParams/" + databaseEnvironmentId, userParams)
      .pipe(map((result: any) => result));
  }
}
