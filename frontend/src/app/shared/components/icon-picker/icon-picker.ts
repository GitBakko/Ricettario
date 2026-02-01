import { Component, input, output, signal, computed, ElementRef, HostListener, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

export interface IconOption {
  value: string;
  label: string;
}

@Component({
  selector: 'app-icon-picker',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './icon-picker.html',
  styleUrl: './icon-picker.scss'
})
export class IconPickerComponent {
  /** List of available icons */
  icons = input.required<IconOption[]>();
  
  /** Currently selected icon value */
  value = input<string>('');
  
  /** Emits when icon selection changes */
  valueChange = output<string>();
  
  /** Placeholder text when no icon selected */
  placeholder = input<string>('Seleziona icona');
  
  /** Preview color for the icon */
  previewColor = input<string>('#6c757d');
  
  /** Dropdown open state */
  isOpen = signal(false);
  
  /** Search filter text */
  searchText = signal('');
  
  @ViewChild('searchInput') searchInput!: ElementRef<HTMLInputElement>;
  
  /** Selected icon object */
  selectedIcon = computed(() => {
    const currentValue = this.value();
    return this.icons().find(i => i.value === currentValue) || null;
  });
  
  /** Filtered icons based on search */
  filteredIcons = computed(() => {
    const search = this.searchText().toLowerCase();
    if (!search) return this.icons();
    return this.icons().filter(icon => 
      icon.label.toLowerCase().includes(search) || 
      icon.value.toLowerCase().includes(search)
    );
  });
  
  constructor(private elementRef: ElementRef) {}
  
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.isOpen.set(false);
    }
  }
  
  toggleDropdown() {
    this.isOpen.update(v => !v);
    if (this.isOpen()) {
      this.searchText.set('');
      setTimeout(() => this.searchInput?.nativeElement?.focus(), 50);
    }
  }
  
  selectIcon(icon: IconOption) {
    this.valueChange.emit(icon.value);
    this.isOpen.set(false);
  }
  
  /** Extract display label without emoji prefix */
  getCleanLabel(label: string): string {
    // Remove emoji prefix if present (e.g., "🍕 Pizza" -> "Pizza")
    return label.replace(/^[\p{Emoji}\s]+/u, '').trim() || label;
  }
}
