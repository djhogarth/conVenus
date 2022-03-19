import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of } from 'rxjs';
import { MapOperator } from 'rxjs/internal/operators/map';
import { map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/members';
import { PaginatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { UserParameters } from '../_models/userParameters';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  baseUrl: string = environment.apiUrl;
  members: Member[] = [];
  user: User;
  memberCache = new Map();
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();
  userParams: UserParameters;


  constructor(private http: HttpClient, private accountService: AccountService ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(currentUser => {
      this.user = currentUser;
      this.userParams = new UserParameters(currentUser);
    })
   }

   getUserParams(){
     return this.userParams;
   }

   setUserParams(params: UserParameters){
     this.userParams = params;
   }

   resetUserParams(){
     this.userParams = new UserParameters(this.user);
     return this.userParams;
   }

  getMembers(userParams: UserParameters) {
    var response = this.memberCache.get(Object.values(userParams).join('-'));
    if(response){
      return of(response);
    }

    let params = getPaginationHeaders(userParams.pageNumber, userParams.pageSize)

    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http,)
    .pipe(map(response => {
      this.memberCache.set(Object.values(userParams).join('-'), response);
      return response;
    }));
}

  getMember(username: string)
  {
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.userName === username);

    if(member) return of(member);

     return this.http.get<Member>(this.baseUrl + 'users/' + username);

  }

  updateMember(member: Member)
  {
    return this.http.put(this.baseUrl + 'users', member).pipe(
    map(() => {
      const index = this.members.indexOf(member);
      this.members[index] = member;
    })
   )
  }

  setMainPhoto(photoId: number)
  {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {})
  }

  deletePhoto(photoId: number)
  {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId);
  }

  //logged in user likes another user
  addLike(username: string)
  {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }
  //returns users liked by logged in user
  getLikes(predicate: string, pageNumber, pageSize)
  {
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    return getPaginatedResult< Partial<Member[]>>(this.baseUrl + 'likes', params, this.http);
  }

}
