import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class SidebarComponent {
  // Input to control visibility on mobile
  isOpen = input<boolean>(false);

  // Output to close sidebar
  close = output<void>();

  onNavItemClick(): void {
    // Close sidebar on mobile after navigation
    this.close.emit();
  }

  onBackdropClick(): void {
    this.close.emit();
  }
}
