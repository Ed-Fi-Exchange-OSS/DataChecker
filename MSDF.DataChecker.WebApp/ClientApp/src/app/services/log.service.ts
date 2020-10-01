import { Injectable } from '@angular/core';
import { UtilService } from './util.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class LogService {

  private component = "logs";
  private url = UtilService.apiUrl() + this.component;

  constructor(private http: HttpClient) { }

  public addLog(log: any): Observable<any> {
    return this.http
      .post(this.url, log)
      .pipe(map((result: any) => result));
  }
}
