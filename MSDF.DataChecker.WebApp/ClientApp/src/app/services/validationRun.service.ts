import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { UtilService } from './util.service';
import { ValidationRun } from '../models/validationRun.model';

@Injectable()
export class ValidationRunService {
  private component = 'ValidationRun';
  private url = UtilService.apiUrl() + this.component;
  private userId = UtilService.currentUserId();

  constructor(private http: HttpClient) { }

  public getValidationRunInfo(validationRun: ValidationRun): Observable<any> {
    return this.http
      .get(this.url + "/" + validationRun.Id, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public addValidationRun(validationRun: ValidationRun): Observable<any> {
    return this.http
      .post(this.url, validationRun)
      .pipe(map((result: any) => result));
  }

  public finishValidationRun(validationRun: ValidationRun): Observable<any> {
    return this.http
      .post(this.url + "/Finish", validationRun)
      .pipe(map((result: any) => result));
  }

  public errorValidationRun(validationRun: ValidationRun): Observable<any> {
    return this.http
      .post(this.url + "/ValidationRunError", validationRun)
      .pipe(map((result: any) => result));
  }
  public getValidationsRun(): Observable<any> {
    return this.http
      .get(this.url, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public updateValidationRun(validationRun: ValidationRun): Observable<any> {
    return this.http
      .post(this.url + "/Update", validationRun)
      .pipe(map((result: any) => result));
  }

  public deleteValidationRun(validationRun: ValidationRun): Observable<any> {
    return this.http
      .get(this.url + "/Delete/" + validationRun.Id, {})
      .pipe(map((result: any) => result));
  }

}
