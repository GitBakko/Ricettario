import { Component, inject, output, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '../../services/auth';
import { SearchService } from '../../core/services/search.service';
import { LanguageService, SupportedLanguage, SUPPORTED_LANGUAGES } from '../../core/services/language.service';
import { ThemeService } from '../../core/services/theme.service';
import { ProfileService } from '../../services/profile';
import { MOCK_NOTIFICATION_COUNT } from '../../core/data/mock-data';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslateModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class NavbarComponent implements OnInit {
  private authService = inject(AuthService);
  private searchService = inject(SearchService);
  private languageService = inject(LanguageService);
  private themeService = inject(ThemeService);
  private profileService = inject(ProfileService);

  // Theme toggle
  isDarkMode = this.themeService.isDark;
  themeIcon = this.themeService.themeIcon;

  // @MOCK: Notification count from mock data
  notificationCount = MOCK_NOTIFICATION_COUNT;
  
  // Profile data from service
  profile = this.profileService.profile;
  
  // Avatar URL computed from profile
  avatarUrl = computed(() => this.profileService.getImageUrl(this.profile()?.avatarUrl));
  
  // Banner URL computed from profile
  bannerUrl = computed(() => this.profileService.getImageUrl(this.profile()?.bannerUrl));
  
  // Initials for fallback avatar
  initials = computed(() => this.profileService.getInitials(this.profile()));

  // Search input bound to service
  searchQuery = signal('');

  // Language selection from service
  currentLanguage = this.languageService.currentLanguage;
  languages = SUPPORTED_LANGUAGES;
  showLanguageDropdown = signal(false);

  // Computed current language info
  currentLangInfo = computed(() => this.languageService.getCurrentLanguageInfo());

  // User dropdown
  showUserDropdown = signal(false);

  // Output event to toggle sidebar (for mobile)
  toggleSidebar = output<void>();

  // Get username from profile or auth service
  get userName(): string {
    return this.profile()?.displayName || this.authService.currentUserName() || 'Utente';
  }
  
  // Get email from profile
  get userEmail(): string {
    return this.profile()?.email || '';
  }

  ngOnInit(): void {
    // Load profile if authenticated
    if (this.authService.isAuthenticated()) {
      this.profileService.getProfile().subscribe();
    }
  }

  onSearchInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchQuery.set(value);
    this.searchService.setSearchQuery(value);
  }

  onSearchSubmit(event: Event): void {
    event.preventDefault();
    // Search is already reactive via the service
  }

  clearSearch(): void {
    this.searchQuery.set('');
    this.searchService.clearSearchQuery();
  }

  toggleFilterPanel(): void {
    this.searchService.toggleFilters();
  }

  setLanguage(lang: SupportedLanguage): void {
    this.languageService.setLanguage(lang);
    this.showLanguageDropdown.set(false);
  }

  toggleLanguageDropdown(): void {
    this.showLanguageDropdown.update(v => !v);
    this.showUserDropdown.set(false);
  }

  toggleUserDropdown(): void {
    this.showUserDropdown.update(v => !v);
    this.showLanguageDropdown.set(false);
  }

  closeDropdowns(): void {
    this.showLanguageDropdown.set(false);
    this.showUserDropdown.set(false);
  }

  logout(): void {
    this.authService.logout();
  }

  onHamburgerClick(): void {
    this.toggleSidebar.emit();
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }
}
