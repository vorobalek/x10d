import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedRoutingModule } from './shared-routing.module';
import { ContainerComponent } from './container/container.component';
import { JBMonoComponent } from './jbmono/jbmono.component';
import { NotFoundComponent } from './not-found/not-found.component';


@NgModule({
  declarations: [
    ContainerComponent,
    JBMonoComponent,
    NotFoundComponent
  ],
  imports: [
    CommonModule,
    SharedRoutingModule
  ]
})
export class SharedModule { }
