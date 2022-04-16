import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { ToastrModule } from 'ngx-toastr';
import {TabsModule} from 'ngx-bootstrap/tabs';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { FileUploadModule } from 'ng2-file-upload';
import { PaginationModule } from "ngx-bootstrap/pagination";
import { ButtonsModule } from "ngx-bootstrap/buttons";
import { ModalModule } from "ngx-bootstrap/modal";
import { TimeagoModule } from "ngx-timeago";

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(),
    ToastrModule.forRoot({
      positionClass:
        'toast-bottom-right'
    }),
    TabsModule.forRoot(),
    NgxGalleryModule,
    FileUploadModule,
    BsDatepickerModule.forRoot(),
    PaginationModule.forRoot(),
    ButtonsModule.forRoot(),
    TimeagoModule.forRoot(),
    ModalModule.forRoot(),

  ],
  exports:[
    ToastrModule,
    BsDropdownModule,
    BsDatepickerModule,
    TabsModule,
    NgxGalleryModule,
    FileUploadModule,
    PaginationModule,
    ButtonsModule,
    TimeagoModule,
    ModalModule,
   ]
})
export class SharedModule { }
