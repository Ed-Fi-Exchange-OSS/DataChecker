import { Injectable } from "@angular/core";
import { NgxSpinnerService } from "ngx-spinner";

import {
  HttpInterceptor,
  HttpRequest,
  HttpResponse,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse
} from "@angular/common/http";

import { Observable, throwError } from "rxjs";
import { map, catchError, finalize } from "rxjs/operators";

@Injectable()
export class HttpConfigInterceptor implements HttpInterceptor {
  constructor(
    private spinner: NgxSpinnerService
  ) { }

  private totalRequests = 0;
  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {

    if (!request.headers.has("Content-Type")) {
      request = request.clone({
        headers: request.headers.set("Content-Type", "application/json")
      });
    }

    request = request.clone({
      headers: request.headers.set("Accept", "application/json")
    });

    this.totalRequests++;
    setTimeout(function () {
      this.spinner.show();
    }.bind(this), 100);

    return next.handle(request).pipe(
      map((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
        }
        return event;
      }),
      catchError((error: HttpErrorResponse) => {
        this.decreaseRequests();
        return throwError(error);
      }),
      finalize(() => this.decreaseRequests())
    );
  }

  private decreaseRequests() {
    this.totalRequests--;
    if (this.totalRequests === 0 || this.totalRequests <= 0) {
      setTimeout(function () {
        this.spinner.hide();
      }.bind(this), 100);
    }
  }
}
