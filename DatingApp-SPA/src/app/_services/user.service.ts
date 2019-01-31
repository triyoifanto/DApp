import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl;

constructor(private Http: HttpClient) { }

getUsers(): Observable<User[]> {
  return this.Http.get<User[]>(this.baseUrl + 'users');
}

getUser(id: number): Observable<User> {
  return this.Http.get<User>(this.baseUrl + 'users/' + id);
}

updateUser(id: number, user: User) {
  return this.Http.put(this.baseUrl + 'users/' + id, user);
}

setMainPhoto(userId: number, id: number) {
  return this.Http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setmain', {});
}

}
