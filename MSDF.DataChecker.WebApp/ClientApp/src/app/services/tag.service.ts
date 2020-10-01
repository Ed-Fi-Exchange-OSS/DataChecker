import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { UtilService } from './util.service';

@Injectable()
export class TagService {

  private component = "tags";
  private url = UtilService.apiUrl() + this.component;

  constructor(private http: HttpClient) { }

  public addTag(tag: any): Observable<any> {
    return this.http
      .post(this.url, tag)
      .pipe(map((result: any) => result));
  }

  public modifyTag(tag: any): Observable<any> {
    return this.http
      .post(this.url + "/Update", tag)
      .pipe(map((result: any) => result));
  }

  public deleteTag(tagId: number): Observable<any> {
    return this.http
      .get(this.url + "/Delete/" + tagId, {})
      .pipe(map((result: any) => result));
  }

  public getByContainer(containerId: string): Observable<any> {
    return this.http
      .get(this.url + "/GetByContainer/" + containerId, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public getByRule(ruleId: string): Observable<any> {
    return this.http
      .get(this.url + "/GetByRule/" + ruleId, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public getById(id: number): Observable<any> {
    return this.http
      .get(this.url + "/" + id, { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public getTags(): Observable<any> {
    return this.http
      .get(this.url + "/", { responseType: "json" })
      .pipe(map((result: any[]) => result));
  }

  public searchByTags(tags: any): Observable<any> {
    return this.http
      .post(this.url +"/SearchByTags", tags)
      .pipe(map((result: any) => result));
  }
}
