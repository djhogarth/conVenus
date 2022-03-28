import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs/internal/Observable';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { ConfirmService } from '../_services/confirm.service';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {

  constructor(private confirmService: ConfirmService)
  {

  }

  canDeactivate(component: MemberEditComponent): Observable<boolean> | boolean {
    if(component.editForm.dirty){
      return confirm('Are you sure you want to leave the page? Any unsaved changes will be lost!')
    }
    return true;
  }

}
