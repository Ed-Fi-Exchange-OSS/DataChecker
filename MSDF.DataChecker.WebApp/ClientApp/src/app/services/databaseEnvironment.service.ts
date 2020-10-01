import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { UtilService } from './util.service';
import { DatabaseEnvironment } from '../models/databaseEnvironment.model';

@Injectable()
export class DatabaseEnvironmentService {
  private component = 'DatabaseEnvironments';
  private url = UtilService.apiUrl() + this.component;
  private userId = UtilService.currentUserId();

  constructor(private http: HttpClient) { }

  public getDatabaseInfo(databaseEnvironment: DatabaseEnvironment): Observable<any> {
    return this.http
      .get(this.url + "/" + databaseEnvironment.id, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public addDatabaseEnvironment(databaseEnvironment: DatabaseEnvironment): Observable<any> {
    databaseEnvironment.createdByUserId = this.userId;
    return this.http
      .post(this.url, databaseEnvironment)
      .pipe(map((result: any) => result));
  }

  public getDatabaseEnvironments(): Observable<any> {
    return this.http
      .get(this.url, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public updateDatabaseEnvironment(databaseEnvironment: DatabaseEnvironment): Observable<any> {
    return this.http
      .post(this.url + "/Update", databaseEnvironment)
      .pipe(map((result: any) => result));
  }

  public deleteDatabaseEnvironment(databaseEnvironment: DatabaseEnvironment): Observable<any> {
    return this.http
      .get(this.url + "/Delete/" + databaseEnvironment.id, {})
      .pipe(map((result: any) => result));
  }

  public duplicateDatabaseEnvironment(databaseEnvironment: DatabaseEnvironment): Observable<any> {
    return this.http
      .post(this.url + "/Duplicate/" + databaseEnvironment.id, {})
      .pipe(map((result: any) => result));
  }

  public testDatabaseEnvironment(databaseEnvironment: DatabaseEnvironment): Observable<any> {
    return this.http
      .post(this.url + "/TestConnection", databaseEnvironment)
      .pipe(map((result: any) => result));
  }

  public testDatabaseEnvironmentById(databaseEnvironment: DatabaseEnvironment): Observable<any> {
    return this.http
      .post(this.url + "/TestConnectionById", databaseEnvironment)
      .pipe(map((result: any) => result));
  }

  public getMaxNumberResults(): Observable<any> {
    return this.http
      .get(this.url + "/GetMaxNumberResults/", {})
      .pipe(map((result: any) => result));
  }
}
