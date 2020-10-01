import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { UtilService } from './util.service';

@Injectable()
export class JobService {

  private component = "jobs";
  private url = UtilService.apiUrl() + this.component;

  constructor(private http: HttpClient) { }

  public addJob(job: any): Observable<any> {
    return this.http
      .post(this.url, job)
      .pipe(map((result: any) => result));
  }

  public modifyJob(job: any): Observable<any> {
    return this.http
      .post(this.url + "/Update", job)
      .pipe(map((result: any) => result));
  }

  public runAndForget(job: any): Observable<any> {
    return this.http
      .post(this.url + "/RunAndForget", job)
      .pipe(map((result: any) => result));
  }

  public deleteJob(jobId: number): Observable<any> {
    return this.http
      .get(this.url + "/Delete/" + jobId, {})
      .pipe(map((result: any) => result));
  }

  public getJobs(): Observable<any> {
    return this.http
      .get(this.url + "/", { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public enqueueJob(jobId: number): Observable<any> {
    return this.http
      .get(this.url + "/Enqueue/" + jobId, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }
}
