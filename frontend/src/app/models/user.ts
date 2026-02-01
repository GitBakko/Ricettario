/**
 * User Profile DTOs for KneadHub
 */

export interface UserProfile {
  id: string;
  email: string;
  displayName?: string;
  bio?: string;
  avatarUrl?: string;
  bannerUrl?: string;
  location?: string;
  website?: string;
  createdAt: Date;
  updatedAt?: Date;
  recipeCount: number;
}

export interface UpdateProfileDto {
  displayName?: string;
  bio?: string;
  location?: string;
  website?: string;
}

export interface UpdateAvatarDto {
  avatarUrl: string;
}
