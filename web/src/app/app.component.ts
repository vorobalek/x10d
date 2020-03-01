import { Component } from '@angular/core';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title: string = 'X10D';
  constructor(private titleService: Title) { }

  onTitleChange() {
    this.titleService.setTitle(this.title);
  }
}
