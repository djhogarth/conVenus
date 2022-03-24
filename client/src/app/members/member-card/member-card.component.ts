import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/members';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})

export class MemberCardComponent implements OnInit {
  @Input() member: Member;

  constructor(private memberService: MembersService, private toastr: ToastrService,
    private presenceService: PresenceService) { }

  ngOnInit(): void
  {

  }

  getPresence(){
    return this.presenceService;
  }

  addLike(member: Member)
  {
    this.memberService.addLike(member.userName).subscribe(() =>
    {
      this.toastr.success('You have liked ' + member.alias);
    })

  }

}
