import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { UtilService } from './util.service';
import { Observable } from "rxjs";
import { map } from "rxjs/operators";

@Injectable({
  providedIn: "root"
})
export class CommunityService {

  private url = UtilService.communityUrl();
  constructor(private http: HttpClient) {}

  public getCommunityCollections(tokenInfo: any): Observable<any> {

    this.url = UtilService.communityUrl();

    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': tokenInfo.type + ' ' + tokenInfo.token
    });

    return this.http
      .get(this.url + "container", { headers })
      .pipe(map((result: any) => result));
  }

  public signInToCommunity(user: any): Observable<any> {
    this.url = UtilService.communityUrl();
    return this.http
      .post(this.url + "oauth/token", user)
      .pipe(map((result: any) => result));
  }

  public addCommunityUser(user: any): Observable<any> {
    this.url = UtilService.communityUrl();
    return this.http
      .post(this.url + "oauth/AddUser", user)
      .pipe(map((result: any) => result));
  }

  public getCollectionByName(collection: any, tokenInfo: any): Observable<any> {

    this.url = UtilService.communityUrl();

    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': tokenInfo.type + ' ' + tokenInfo.token
    });

    return this.http
      .post(this.url + "container/GetByName", collection, { headers })
      .pipe(map((result: any) => result));
  }

  public uploadCollection(collection: any, tokenInfo: any): Observable<any> {

    this.url = UtilService.communityUrl();

    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': tokenInfo.type + ' ' + tokenInfo.token
    });

    return this.http
      .post(this.url + "container/Upload", collection, { headers })
      .pipe(map((result: any) => result));
  }

  public isTokenValid(tokenInfo: any): Observable<any> {

    this.url = UtilService.communityUrl();

    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': tokenInfo.type + ' ' + tokenInfo.token
    });

    return this.http
      .post(this.url + "oauth/IsAuthenticated", { email: 'test1', password: 'test2' }, { headers })
      .pipe(map((result: any) => result));
  }

  public validateDestinationTable(info: any, tokenInfo: any): Observable<any> {

    this.url = UtilService.communityUrl();

    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': tokenInfo.type + ' ' + tokenInfo.token
    });

    return this.http
      .post(this.url + "container/ValidateDestinationTable", info, { headers })
      .pipe(map((result: any) => result));
  }
}
