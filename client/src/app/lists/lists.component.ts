import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/members';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  members: Partial<Member[]>;
  predicate = 'liked';

  constructor(private memberService: MembersService)
  {
    this.loadLikes();
  }

  ngOnInit(): void
  {

  }

  loadLikes()
  {
    this.memberService.getLikes(this.predicate).subscribe(response =>
      {
        this.members = response;
      })
  }

}
