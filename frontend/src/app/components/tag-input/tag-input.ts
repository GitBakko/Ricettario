import { Component, EventEmitter, Input, OnInit, Output, signal, inject, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TagService } from '../../services/tag';
import { Tag } from '../../models/tag';
import { Subject, debounceTime, distinctUntilChanged, switchMap, of } from 'rxjs';

@Component({
  selector: 'app-tag-input',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tag-input.html',
  styleUrl: './tag-input.scss',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TagInputComponent),
      multi: true
    }
  ]
})
export class TagInputComponent implements OnInit, ControlValueAccessor {
  private tagService = inject(TagService);

  @Input() placeholder = 'Cerca o crea tags...';
  @Output() tagsChanged = new EventEmitter<number[]>();

  selectedTags = signal<Tag[]>([]);
  suggestions = signal<Tag[]>([]);
  allTags = signal<Tag[]>([]);
  showSuggestions = signal(false);
  searchQuery = '';
  isLoading = signal(false);

  private searchSubject = new Subject<string>();
  private onChange: (value: number[]) => void = () => {};
  private onTouched: () => void = () => {};

  ngOnInit(): void {
    // Load all tags initially
    this.tagService.getTags().subscribe(tags => {
      this.allTags.set(tags);
    });

    // Setup debounced search
    this.searchSubject.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      switchMap(query => {
        if (!query.trim()) {
          return of(this.allTags());
        }
        return this.tagService.getTags(query);
      })
    ).subscribe(tags => {
      this.updateSuggestions(tags);
    });
  }

  private updateSuggestions(tags: Tag[]): void {
    // Filter out already selected tags
    const selectedIds = new Set(this.selectedTags().map(t => t.id));
    this.suggestions.set(tags.filter(t => !selectedIds.has(t.id)));
  }

  onSearchChange(): void {
    this.showSuggestions.set(true);
    this.searchSubject.next(this.searchQuery);
  }

  onFocus(): void {
    this.showSuggestions.set(true);
    this.updateSuggestions(this.allTags());
  }

  onBlur(): void {
    // Delay to allow click on suggestion
    setTimeout(() => {
      this.showSuggestions.set(false);
    }, 200);
    this.onTouched();
  }

  selectTag(tag: Tag): void {
    const current = this.selectedTags();
    if (!current.find(t => t.id === tag.id)) {
      this.selectedTags.set([...current, tag]);
      this.emitChange();
    }
    this.searchQuery = '';
    this.updateSuggestions(this.allTags());
  }

  removeTag(tag: Tag): void {
    this.selectedTags.set(this.selectedTags().filter(t => t.id !== tag.id));
    this.emitChange();
    this.updateSuggestions(this.allTags());
  }

  createNewTag(): void {
    if (!this.searchQuery.trim()) return;

    this.isLoading.set(true);
    this.tagService.createTag({ name: this.searchQuery.trim() }).subscribe({
      next: (newTag) => {
        this.selectTag(newTag);
        // Refresh all tags
        this.tagService.getTags().subscribe(tags => {
          this.allTags.set(tags);
          this.updateSuggestions(tags);
        });
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error creating tag:', err);
        this.isLoading.set(false);
      }
    });
  }

  onKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter') {
      event.preventDefault();
      if (this.suggestions().length > 0) {
        this.selectTag(this.suggestions()[0]);
      } else if (this.searchQuery.trim()) {
        this.createNewTag();
      }
    }
  }

  private emitChange(): void {
    const ids = this.selectedTags().map(t => t.id);
    this.onChange(ids);
    this.tagsChanged.emit(ids);
  }

  // ControlValueAccessor implementation
  writeValue(tagIds: number[]): void {
    if (tagIds && tagIds.length > 0) {
      // Load tags by IDs
      const allTags = this.allTags();
      if (allTags.length > 0) {
        const selected = allTags.filter(t => tagIds.includes(t.id));
        this.selectedTags.set(selected);
      } else {
        // Wait for tags to load
        this.tagService.getTags().subscribe(tags => {
          this.allTags.set(tags);
          const selected = tags.filter(t => tagIds.includes(t.id));
          this.selectedTags.set(selected);
        });
      }
    } else {
      this.selectedTags.set([]);
    }
  }

  registerOnChange(fn: (value: number[]) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  get hasNewTagOption(): boolean {
    const query = this.searchQuery.trim().toLowerCase();
    if (!query) return false;
    return !this.allTags().some(t => t.name.toLowerCase() === query);
  }
}
