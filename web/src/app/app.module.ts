import { BrowserModule, Title } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms'
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { JbmonoComponent } from './shared/components/jbmono/jbmono.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { ContainerComponent } from './shared/components/container/container.component';

@NgModule({
  declarations: [
    AppComponent,
    JbmonoComponent,
    NotFoundComponent,
    ContainerComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule
  ],
  providers: [
    Title
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
