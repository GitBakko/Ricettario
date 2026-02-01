import { Component, computed, inject, OnInit, signal, ViewChild, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ProfileService } from '../../services/profile';
import { AuthService } from '../../services/auth';
import { UserProfile, UpdateProfileDto } from '../../models/user';

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './profile-page.component.html',
  styleUrl: './profile-page.component.scss'
})
export class ProfilePageComponent implements OnInit {
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;
  @ViewChild('bannerInput') bannerInput!: ElementRef<HTMLInputElement>;

  private profileService = inject(ProfileService);
  private authService = inject(AuthService);

  // State
  isLoading = signal(true);
  isSaving = signal(false);
  isUploadingAvatar = signal(false);
  isUploadingBanner = signal(false);
  error = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  isEditing = signal(false);

  // Profile data
  profile = this.profileService.profile;

  // Edit form model
  editForm = signal<UpdateProfileDto>({
    displayName: '',
    bio: '',
    location: '',
    website: ''
  });

  // Computed
  initials = computed(() => this.profileService.getInitials(this.profile()));
  avatarUrl = computed(() => this.profileService.getImageUrl(this.profile()?.avatarUrl));
  bannerUrl = computed(() => this.profileService.getImageUrl(this.profile()?.bannerUrl));
  memberSince = computed(() => {
    const p = this.profile();
    if (!p?.createdAt) return '';
    const date = new Date(p.createdAt);
    return date.toLocaleDateString('it-IT', { month: 'long', year: 'numeric' });
  });

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.profileService.getProfile().subscribe({
      next: (profile) => {
        this.isLoading.set(false);
        this.resetEditForm(profile);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.error.set(err?.error?.message || 'Errore nel caricamento del profilo');
      }
    });
  }

  startEditing(): void {
    const p = this.profile();
    if (p) {
      this.resetEditForm(p);
    }
    this.isEditing.set(true);
    this.successMessage.set(null);
  }

  cancelEditing(): void {
    this.isEditing.set(false);
    const p = this.profile();
    if (p) {
      this.resetEditForm(p);
    }
  }

  saveProfile(): void {
    this.isSaving.set(true);
    this.error.set(null);

    this.profileService.updateProfile(this.editForm()).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.isEditing.set(false);
        this.successMessage.set('Profilo aggiornato con successo!');
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.isSaving.set(false);
        this.error.set(err?.error?.message || 'Errore nel salvataggio del profilo');
      }
    });
  }

  triggerAvatarUpload(): void {
    this.fileInput.nativeElement.click();
  }

  onAvatarSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    const file = input.files[0];
    
    // Validate file type
    if (!file.type.startsWith('image/')) {
      this.error.set('Seleziona un file immagine valido');
      return;
    }

    // Validate file size (5MB max)
    if (file.size > 5 * 1024 * 1024) {
      this.error.set('L\'immagine non può superare i 5MB');
      return;
    }

    this.uploadAvatar(file);
  }

  private uploadAvatar(file: File): void {
    this.isUploadingAvatar.set(true);
    this.error.set(null);

    this.profileService.uploadAvatar(file).subscribe({
      next: () => {
        this.isUploadingAvatar.set(false);
        this.successMessage.set('Avatar aggiornato!');
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.isUploadingAvatar.set(false);
        this.error.set(err?.error?.message || 'Errore nel caricamento dell\'avatar');
      }
    });
  }

  private resetEditForm(profile: UserProfile): void {
    this.editForm.set({
      displayName: profile.displayName || '',
      bio: profile.bio || '',
      location: profile.location || '',
      website: profile.website || ''
    });
  }

  updateFormField(field: keyof UpdateProfileDto, value: string): void {
    this.editForm.update(form => ({
      ...form,
      [field]: value
    }));
  }

  // Banner upload methods
  triggerBannerUpload(): void {
    this.bannerInput.nativeElement.click();
  }

  onBannerSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    const file = input.files[0];
    
    // Validate file type
    if (!file.type.startsWith('image/')) {
      this.error.set('Seleziona un file immagine valido');
      return;
    }

    // Validate file size (10MB max for banners)
    if (file.size > 10 * 1024 * 1024) {
      this.error.set('L\'immagine banner non può superare i 10MB');
      return;
    }

    this.uploadBanner(file);
  }

  private uploadBanner(file: File): void {
    this.isUploadingBanner.set(true);
    this.error.set(null);

    this.profileService.uploadBanner(file).subscribe({
      next: () => {
        this.isUploadingBanner.set(false);
        this.successMessage.set('Banner aggiornato!');
        setTimeout(() => this.successMessage.set(null), 3000);
      },
      error: (err) => {
        this.isUploadingBanner.set(false);
        this.error.set(err?.error?.message || 'Errore nel caricamento del banner');
      }
    });
  }
}
