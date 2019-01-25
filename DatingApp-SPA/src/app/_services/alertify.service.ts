import { Injectable } from '@angular/core';
declare let alertify: any;

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {

  constructor() { }

  // confirm alertify method
  confirm(message: string, okCallback: () => any) {
    alertify.confirm(message, function(e) {
      if (e) {
        okCallback();
      } else {}
    });
  }

  // success alertify method
  success(message: string) {
    alertify.success(message);
  }

  // error alertify method
  error(message: string) {
    alertify.error(message);
  }

  // warning alertify method
  warning(message: string) {
    alertify.warning(message);
  }

  // message alertify method
  message(message: string) {
    alertify.message(message);
  }
}
