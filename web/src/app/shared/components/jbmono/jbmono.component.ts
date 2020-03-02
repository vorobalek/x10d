import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-jbmono',
  templateUrl: './jbmono.component.html',
  styleUrls: ['./jbmono.component.scss']
})
export class JbmonoComponent implements OnInit {
  @Input() type: string;
  @Input() size: string;
  class: string = 'app-jbmono-r';
  constructor() { }
  ngOnInit() {
    if (this.type != null) {
      let classPrefix = 'app-jbmono-';
      let classPostfix = 'r';
      switch (this.type.toLowerCase()) {
        case 'bold-italic':
        case 'bold,italic':
        case 'bold, italic':
        case 'bold italic':
        case 'italic-bold':
        case 'italic,bold':
        case 'italic, bold':
        case 'italic bold':
          classPostfix = 'bi';
          break;

        case 'bold':
          classPostfix = 'b';
          break;

        case 'extra-bold-italic':
        case 'extra,bold,italic':
        case 'extra, bold, italic':
        case 'extra bold italic':
        case 'extrabold-italic':
        case 'extrabold,italic':
        case 'extrabold, italic':
        case 'extrabold italic':
        case 'italic-extra-bold':
        case 'italic,extra,bold':
        case 'italic, extra, bold':
        case 'italic extra bold ':
        case 'italic-extrabold':
        case 'italic,extrabold':
        case 'italic, extrabold':
        case 'italic extrabold ':
          classPostfix = 'xbi';
          break;

        case 'extra-bold':
        case 'extra,bold':
        case 'extra, bold':
        case 'extra bold':
        case 'extrabold':
          classPostfix = 'xb';
          break;

        case 'italic':
          classPostfix = 'i';
          break;

        case 'medium-italic':
        case 'medium,italic':
        case 'medium, italic':
        case 'medium italic':
        case 'italic-medium':
        case 'italic,medium':
        case 'italic, medium':
        case 'italic medium':
          classPostfix = 'mi';
          break;

        case 'medium':
          classPostfix = 'm';
          break;

        case 'regular':
          classPostfix = 'r';
          break;

        default:
          classPostfix = 'r';
          break;
      }
      this.class = classPrefix + classPostfix;
    }
  }
}
