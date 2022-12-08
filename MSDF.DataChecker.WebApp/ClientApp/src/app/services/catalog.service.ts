import { Inject, Injector, Injectable  } from '@angular/core';
import { UtilService } from './util.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class CatalogService {

  private component = "catalogs";
  private url = UtilService.apiUrl() + this.component;

  constructor(private http: HttpClient) {
  }
/*  constructor(private http: HttpClient) { }*/

  public get(): Observable<any> {
    return this.http
      .get(this.url + "/", { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public getByType(catalogType: string): Observable<any> {
    return this.http
      .get(this.url + "/GetByType?type=" + catalogType, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public getById(id: number): Observable<any> {
    return this.http
      .get(this.url + "/" + id, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }
}
