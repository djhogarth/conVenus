import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/members';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  user: User;
  member: Member;
  @ViewChild('editForm') editForm: NgForm;

  constructor(private accountService: AccountService, private memberService: MembersService, private toastr: ToastrService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(_user => this.user = _user);
  }

  ngOnInit(): void {
    this?.loadMember();

  }

  loadMember(){
    this.memberService.getMember(this.user.username).subscribe(_member => this.member = _member);
  }

  updateMember(){
    this.toastr.success("Profile Updated")
    this.editForm.reset(this.member);
  }
}
