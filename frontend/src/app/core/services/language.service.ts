import { Injectable, inject, signal } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

export type SupportedLanguage = 'it' | 'en';

export interface LanguageOption {
  code: SupportedLanguage;
  label: string;
  flag: string;
}

// Available languages
export const SUPPORTED_LANGUAGES: LanguageOption[] = [
  { code: 'it', label: 'Italiano', flag: '🇮🇹' },
  { code: 'en', label: 'English', flag: '🇬🇧' }
];

const STORAGE_KEY = 'kneadhub_language';

@Injectable({
  providedIn: 'root'
})
export class LanguageService {
  private translate = inject(TranslateService);
  
  // Current language as signal for reactivity
  currentLanguage = signal<SupportedLanguage>(this.getInitialLanguage());
  
  // Available languages
  readonly languages = SUPPORTED_LANGUAGES;

  constructor() {
    this.initializeLanguage();
  }

  private getInitialLanguage(): SupportedLanguage {
    // Check localStorage first
    const stored = localStorage.getItem(STORAGE_KEY) as SupportedLanguage;
    if (stored && this.isValidLanguage(stored)) {
      return stored;
    }

    // Default to Italian (app is primarily for Italian users)
    // User can switch to English manually if needed
    return 'it';
  }

  private isValidLanguage(lang: string): boolean {
    return SUPPORTED_LANGUAGES.some(l => l.code === lang);
  }

  private initializeLanguage(): void {
    const lang = this.currentLanguage();
    this.translate.setDefaultLang('it');
    this.translate.use(lang);
  }

  /**
   * Switch to a different language
   */
  setLanguage(lang: SupportedLanguage): void {
    if (!this.isValidLanguage(lang)) {
      console.warn(`Language '${lang}' is not supported`);
      return;
    }

    this.currentLanguage.set(lang);
    this.translate.use(lang);
    localStorage.setItem(STORAGE_KEY, lang);
  }

  /**
   * Toggle between IT and EN
   */
  toggleLanguage(): void {
    const current = this.currentLanguage();
    const next: SupportedLanguage = current === 'it' ? 'en' : 'it';
    this.setLanguage(next);
  }

  /**
   * Get current language info
   */
  getCurrentLanguageInfo(): LanguageOption {
    const code = this.currentLanguage();
    return SUPPORTED_LANGUAGES.find(l => l.code === code) || SUPPORTED_LANGUAGES[0];
  }
}
