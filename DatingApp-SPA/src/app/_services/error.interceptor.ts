import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
// get error message from response header
export class ErrorInterceptor implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError(error => {
                if (error instanceof HttpErrorResponse) {
                    // interceptor for unauthorized error
                    if (error.status === 401) {
                        return throwError(error.statusText);
                    }

                    // interceptor for application error internalserver error(500), that we already add the handler in API global exception
                    const applicationError = error.headers.get('Application-Error');
                    if (applicationError) {
                        console.log(applicationError);
                        return throwError(applicationError);
                    }

                    // get server error, bad request(400)
                    const serverError = error.error;
                    let modelStateError = '';
                    // interceptor handle for modelstate error
                    if (serverError && typeof serverError === 'object') {
                        for (const key in serverError) {
                            if (serverError[key]) {
                                modelStateError += serverError[key] + '\n';
                            }
                        }
                    }

                    // return the error either it's modelstate, server, or unpredicted error
                    return throwError(modelStateError || serverError || 'server error');
                }
            })
        );
    }
}

// register this provider to app.module providers
export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
};
