import { ChangeDetectionStrategy, Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Message } from 'src/app/_models/message';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() messages: Message[];
  @Input() username: string;
  @ViewChild('messageForm') messageForm: NgForm;
  messageContent: string;

  constructor(private messageService: MessageService) { }

  ngOnInit(): void
  {

  }

  //get method for message service
  getMessageService()
  {
    return this.messageService;
  }

  sendMessage()
  {
    this.messageService.sendMessage(this.username, this.messageContent).then(() => {
      this.messageForm.reset();
    })
  }



}
