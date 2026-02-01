import { Injectable, signal, computed, effect, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

// @MOCK: Theme types
export type ThemeMode = 'light' | 'dark';

const THEME_STORAGE_KEY = 'kneadhub-theme';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private platformId = inject(PLATFORM_ID);

  // Signal for reactive theme state
  private _theme = signal<ThemeMode>(this.getInitialTheme());

  // Public readonly signal
  readonly theme = this._theme.asReadonly();

  // Computed signals for convenience
  readonly isDark = computed(() => this._theme() === 'dark');
  readonly isLight = computed(() => this._theme() === 'light');

  // Theme icon for toggle button
  readonly themeIcon = computed(() => this._theme() === 'dark' ? 'fa-sun' : 'fa-moon');

  constructor() {
    // Effect to persist theme and apply to DOM
    effect(() => {
      const currentTheme = this._theme();
      if (isPlatformBrowser(this.platformId)) {
        this.applyTheme(currentTheme);
        this.persistTheme(currentTheme);
      }
    });
  }

  /**
   * Toggle between light and dark themes
   */
  toggleTheme(): void {
    this._theme.update(current => current === 'light' ? 'dark' : 'light');
  }

  /**
   * Set a specific theme
   */
  setTheme(theme: ThemeMode): void {
    this._theme.set(theme);
  }

  /**
   * Get the initial theme from localStorage or system preference
   */
  private getInitialTheme(): ThemeMode {
    if (!isPlatformBrowser(this.platformId)) {
      return 'light';
    }

    // Check localStorage first
    const stored = localStorage.getItem(THEME_STORAGE_KEY) as ThemeMode | null;
    if (stored && (stored === 'light' || stored === 'dark')) {
      return stored;
    }

    // Fall back to system preference
    if (window.matchMedia?.('(prefers-color-scheme: dark)').matches) {
      return 'dark';
    }

    return 'light';
  }

  /**
   * Apply theme to the document
   */
  private applyTheme(theme: ThemeMode): void {
    const root = document.documentElement;

    // Remove both classes first
    root.classList.remove('theme-light', 'theme-dark');

    // Add the current theme class
    root.classList.add(`theme-${theme}`);

    // Also set data attribute for CSS selectors
    root.setAttribute('data-theme', theme);

    // Update meta theme-color for mobile browsers
    const metaThemeColor = document.querySelector('meta[name="theme-color"]');
    if (metaThemeColor) {
      metaThemeColor.setAttribute('content', theme === 'dark' ? '#0F0A05' : '#F7F5F6');
    }
  }

  /**
   * Persist theme to localStorage
   */
  private persistTheme(theme: ThemeMode): void {
    localStorage.setItem(THEME_STORAGE_KEY, theme);
  }
}
