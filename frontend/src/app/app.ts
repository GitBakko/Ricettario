import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from './services/auth';
import { NavbarComponent } from './components/navbar/navbar';
import { SidebarComponent } from './components/sidebar/sidebar';
import { FeaturedBakerComponent } from './components/featured-baker/featured-baker';
import { TrendingNowComponent } from './components/trending-now/trending-now';
import { BottomNavComponent } from './components/bottom-nav/bottom-nav';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet, 
    CommonModule,
    NavbarComponent,
    SidebarComponent,
    FeaturedBakerComponent,
    TrendingNowComponent,
    BottomNavComponent
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  authService = inject(AuthService);
  
  // Sidebar state for mobile
  sidebarOpen = signal(false);

  toggleSidebar(): void {
    this.sidebarOpen.update(v => !v);
  }

  closeSidebar(): void {
    this.sidebarOpen.set(false);
  }
  
  logout() {
    this.authService.logout();
  }
}
