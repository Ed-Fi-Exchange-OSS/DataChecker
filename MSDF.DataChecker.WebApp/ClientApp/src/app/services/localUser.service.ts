import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { UtilService } from './util.service';

@Injectable({
  providedIn: "root"
})
export class LocalUserService {

  private component = "communityuser";
  private url = UtilService.apiUrl() + this.component;
  private userId = UtilService.currentUserId();

  constructor(private http: HttpClient) { }

  public getLocalUser(id: string = ""): Observable<any> {
    return this.http
      .get(this.url + "/" + this.userId, { responseType: "json" })
      .pipe(map((result: any) => result));
  }
  public updateUserInformation(localUser: any): Observable<any> {
    return this.http
      .post(this.url + "/Update", localUser)
      .pipe(map((result: any) => result));
  }
}
