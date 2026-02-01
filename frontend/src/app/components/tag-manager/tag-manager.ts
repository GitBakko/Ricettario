import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TagService } from '../../services/tag';
import { Tag, TagCreate, TagUpdate } from '../../models/tag';

@Component({
  selector: 'app-tag-manager',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './tag-manager.html',
  styleUrl: './tag-manager.scss'
})
export class TagManagerComponent implements OnInit {
  private tagService = inject(TagService);

  tags = signal<Tag[]>([]);
  filteredTags = signal<Tag[]>([]);
  loading = signal(false);
  editingId = signal<number | null>(null);
  searchQuery = '';

  formModel: TagCreate = {
    name: '',
    color: this.generateRandomColor()
  };

  ngOnInit(): void {
    this.loadTags();
  }

  loadTags(): void {
    this.loading.set(true);
    this.tagService.getTags().subscribe({
      next: (data) => {
        this.tags.set(data);
        this.filterTags();
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Error loading tags:', err);
        this.loading.set(false);
      }
    });
  }

  filterTags(): void {
    const query = this.searchQuery.toLowerCase().trim();
    if (!query) {
      this.filteredTags.set(this.tags());
    } else {
      this.filteredTags.set(
        this.tags().filter(t => t.name.toLowerCase().includes(query))
      );
    }
  }

  onSearchChange(): void {
    this.filterTags();
  }

  edit(tag: Tag): void {
    this.editingId.set(tag.id);
    this.formModel = {
      name: tag.name,
      color: tag.color || this.generateRandomColor()
    };
  }

  cancel(): void {
    this.editingId.set(null);
    this.resetForm();
  }

  resetForm(): void {
    this.formModel = {
      name: '',
      color: this.generateRandomColor()
    };
  }

  save(): void {
    if (!this.formModel.name.trim()) return;

    if (this.editingId()) {
      const updateDto: TagUpdate = {
        name: this.formModel.name,
        color: this.formModel.color
      };

      this.tagService.updateTag(this.editingId()!, updateDto).subscribe({
        next: () => {
          this.loadTags();
          this.cancel();
        },
        error: (err) => console.error('Error updating tag:', err)
      });
    } else {
      this.tagService.createTag(this.formModel).subscribe({
        next: () => {
          this.loadTags();
          this.resetForm();
        },
        error: (err) => console.error('Error creating tag:', err)
      });
    }
  }

  delete(tag: Tag): void {
    if (!confirm(`Sei sicuro di voler eliminare il tag "${tag.name}"?`)) return;

    this.tagService.deleteTag(tag.id).subscribe({
      next: () => this.loadTags(),
      error: (err) => console.error('Error deleting tag:', err)
    });
  }

  generateRandomColor(): string {
    const colors = [
      '#ef4444', '#f97316', '#f59e0b', '#eab308', '#84cc16',
      '#22c55e', '#10b981', '#14b8a6', '#06b6d4', '#0ea5e9',
      '#3b82f6', '#6366f1', '#8b5cf6', '#a855f7', '#d946ef',
      '#ec4899', '#f43f5e'
    ];
    return colors[Math.floor(Math.random() * colors.length)];
  }

  regenerateColor(): void {
    this.formModel.color = this.generateRandomColor();
  }
}
