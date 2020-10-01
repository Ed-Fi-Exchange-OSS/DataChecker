import { ErrorHandler, Inject, Injector, Injectable } from "@angular/core";
import { HttpErrorResponse } from '@angular/common/http'
import { LogService } from './log.service';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class GlobalErrorsService extends ErrorHandler {

  constructor(@Inject(Injector) private injector: Injector) {
    super();
  }

  // Need to get ToastrService from injector rather than constructor injection to avoid cyclic dependency error
  private get toastrService(): ToastrService {
    return this.injector.get(ToastrService);
  }

  private get logService(): LogService {
    return this.injector.get(LogService);
  }

  public handleError(error: any): void {

    console.log('handleError');
    console.log(error);

    let errorObject = {
      Message: '',
      MessageString: '',
      MessageStack: '',
      ErrorStatus: '',
      ErrorTitle: '',
      ErrorStatusText: '',
      Error: null
    };

    let logModel = {
      Source: 'Web',
      Information: ''
    };

    if (error instanceof HttpErrorResponse) {
      if (error != null) {
        errorObject.MessageString = `${error.status} - ${error.statusText || ''}`;
        if (error.error != null) {
          if (error.error.text != undefined && error.error.text != null)
            errorObject.Message = `${error.message} - ${error.error.message || error.error.text}`;
          else
            errorObject.Message = `${error.message} - ${error.error.message || error.error.toString()}`;

          if (error.error.errors) {
            let allErrors = '';
            for (let key in error.error.errors) {
              allErrors += error.error.errors[key][0] + ' ';
            }
            errorObject.Message = `${error.message} - ${allErrors}`;
          }
          errorObject.ErrorStatus = error.error.status;
          errorObject.ErrorTitle = error.error.title;
          errorObject.ErrorStatusText = error.error.statusText;
          errorObject.Error = error;
        }
        else {
          errorObject.Message = `${error.message}`;
        }
      }
    }
    else {
      errorObject.Message = error.message;
      errorObject.MessageString = error.toString();
      errorObject.MessageStack = error.stack;
    }

    this.toastrService.error(errorObject.Message, "Error", {
      closeButton: true,
      timeOut: 5000,
      onActivateTick: true
    });

    try {
      logModel.Information = JSON.stringify(errorObject);
      this.logService.addLog(logModel).subscribe(rec => { });
    }
    catch (errorInCatch) {
      console.log('Error in Catch');
      console.log(errorInCatch);
    }

    super.handleError(error);
  }
}
