import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { map } from "rxjs/operators";
import { UtilService } from './util.service';
import { Category } from "../models/category.model";

@Injectable()
export class ContainerService {
  private component = "Containers";
  private url = UtilService.apiUrl() + this.component;
  private userId = UtilService.currentUserId();

  constructor(private http: HttpClient) { }

  public getContainerByContainerIdAndDatabaseEnvironmentId(containerId: string = "", databaseEnvironmentId: string = ""): Observable<any> {
    return this.http
      .get(this.url + "/" + containerId + "/details/" + databaseEnvironmentId, {
        responseType: "json"
      })
      .pipe(map((result: any) => result));
  }

  public getAllCollections(): Observable<any> {
    return this.http
      .get(this.url, { responseType: "json" })
      .pipe(map((result: any) => result));
  }

  public getChildContainers(): Observable<any> {
    return this.http
      .get(this.url + '/ChildContainers', { responseType: "json" })
      .pipe(map((result: any) => result));
  }

  public getParentContainers(): Observable<any> {
    return this.http
      .get(this.url + '/ParentContainers', { responseType: "json" })
      .pipe(map((result: any) => result));
  }

  public addContainerFromCommunity(collection: Category): Observable<any> {
    return this.http
      .post(this.url + "/AddContainerFromCommunity", collection)
      .pipe(map((result: any) => result));
  }

  public addCollectionContainer(collection: Category): Observable<any> {
    collection.createdByUserId = this.userId;
    collection.containerTypeId = 1;
    return this.http
      .post(this.url, collection)
      .pipe(map((result: any) => result));
  }

  public addContainer(collection: Category): Observable<any> {
    collection.createdByUserId = this.userId;
    collection.containerTypeId = 2;
    return this.http
      .post(this.url, collection)
      .pipe(map((result: any) => result));
  }

  public deleteContainer(containerId: string): Observable<any> {
    return this.http
      .get(this.url + "/Delete/" + containerId, {})
      .pipe(map((result: any) => result));
  }

  public updateContainer(container: Category): Observable<any> {
    return this.http
      .post(this.url + "/Update", container)
      .pipe(map((result: any) => result));
  }

  public setDefaultContainer(container: Category): Observable<any> {
    return this.http
      .post(this.url + "/SetDefaultAsync", container)
      .pipe(map((result: any) => result));
  }

  public getContainerToCommunity(containerId: string): Observable<any> {
    return this.http
      .get(this.url + "/GetToCommunity/" + containerId, {
        responseType: "json"
      })
      .pipe(map((result: any) => result));
  }

  public getContainerByName(collection: Category): Observable<any> {
    return this.http
      .post(this.url + "/GetByName", collection)
      .pipe(map((result: any) => result));
  }

  public validateDestinationTable(info: any): Observable<any> {
    return this.http
      .post(this.url + "/ValidateDestinationTable", info)
      .pipe(map((result: any) => result));
  }
}
