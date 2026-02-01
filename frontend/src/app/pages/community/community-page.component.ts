import { Component } from '@angular/core';
import { ComingSoonComponent } from '../../components/coming-soon/coming-soon.component';

@Component({
  selector: 'app-community-page',
  standalone: true,
  imports: [ComingSoonComponent],
  template: `
    <app-coming-soon 
      icon="fa-users" 
      pageName="Community">
    </app-coming-soon>
  `
})
export class CommunityPageComponent {}
