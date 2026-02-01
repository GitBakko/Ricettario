// =============================================================================
// KneadHub Mock Data
// =============================================================================
// All mock data is centralized here for easy tracking and future replacement
// Each constant is marked with @MOCK: comment for easy identification
// =============================================================================

// -----------------------------------------------------------------------------
// Interfaces
// -----------------------------------------------------------------------------

export interface FeaturedBaker {
  id: number;
  name: string;
  avatar: string;
  rating: number;
  recipesCount: number;
  specialty: string;
}

export interface TrendingRecipe {
  id: number;
  title: string;
  thumbnail: string;
  author: string;
  likes: number;
}

export interface NavItem {
  id: string;
  labelKey: string; // i18n translation key
  icon: string;
  route: string;
  badge?: number;
}

// -----------------------------------------------------------------------------
// @MOCK: Navigation Items
// Used in: SidebarComponent, BottomNavComponent
// -----------------------------------------------------------------------------
export const MOCK_NAV_ITEMS: NavItem[] = [
  {
    id: 'home',
    labelKey: 'SIDEBAR.HOME',
    icon: 'fa-home',
    route: '/'
  },
  {
    id: 'recipes',
    labelKey: 'SIDEBAR.RECIPES',
    icon: 'fa-utensils',
    route: '/'
  },
  {
    id: 'community',
    labelKey: 'SIDEBAR.COMMUNITY',
    icon: 'fa-users',
    route: '/community'
  },
  {
    id: 'profile',
    labelKey: 'SIDEBAR.PROFILE',
    icon: 'fa-user',
    route: '/profile'
  }
];

// -----------------------------------------------------------------------------
// @MOCK: Featured Bakers
// Used in: FeaturedBakerComponent
// These are highlighted bakers shown in the right sidebar
// -----------------------------------------------------------------------------
export const MOCK_FEATURED_BAKERS: FeaturedBaker[] = [
  {
    id: 1,
    name: 'Marco Rossi',
    avatar: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=100&h=100&fit=crop&crop=face',
    rating: 4.9,
    recipesCount: 47,
    specialty: 'Pane a lievitazione naturale'
  },
  {
    id: 2,
    name: 'Giulia Bianchi',
    avatar: 'https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=100&h=100&fit=crop&crop=face',
    rating: 4.8,
    recipesCount: 32,
    specialty: 'Focacce regionali'
  },
  {
    id: 3,
    name: 'Alessandro Verdi',
    avatar: 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=100&h=100&fit=crop&crop=face',
    rating: 4.7,
    recipesCount: 28,
    specialty: 'Pizza napoletana'
  }
];

// -----------------------------------------------------------------------------
// @MOCK: Trending Recipes
// Used in: TrendingNowComponent
// Popular recipes shown in the right sidebar
// -----------------------------------------------------------------------------
export const MOCK_TRENDING_RECIPES: TrendingRecipe[] = [
  {
    id: 101,
    title: 'Ciabatta Classica',
    thumbnail: 'https://images.unsplash.com/photo-1549931319-a545dcf3bc73?w=120&h=80&fit=crop',
    author: 'Marco Rossi',
    likes: 234
  },
  {
    id: 102,
    title: 'Focaccia Genovese',
    thumbnail: 'https://images.unsplash.com/photo-1619535860434-ba1d8fa12536?w=120&h=80&fit=crop',
    author: 'Giulia Bianchi',
    likes: 189
  },
  {
    id: 103,
    title: 'Pane di Altamura',
    thumbnail: 'https://images.unsplash.com/photo-1509440159596-0249088772ff?w=120&h=80&fit=crop',
    author: 'Alessandro Verdi',
    likes: 156
  },
  {
    id: 104,
    title: 'Grissini Torinesi',
    thumbnail: 'https://images.unsplash.com/photo-1568471173242-461f0a730452?w=120&h=80&fit=crop',
    author: 'Sofia Neri',
    likes: 142
  }
];

// -----------------------------------------------------------------------------
// @MOCK: Notification Count
// Used in: NavbarComponent (bell icon badge)
// Number of unread notifications for current user
// -----------------------------------------------------------------------------
export const MOCK_NOTIFICATION_COUNT: number = 3;

// -----------------------------------------------------------------------------
// @MOCK: Recipe Badges
// Used in: RecipeCard
// Map of recipe IDs to their badge type ('new' | 'popular' | null)
// In production, this would be calculated based on creation date and like count
// -----------------------------------------------------------------------------
export const MOCK_RECIPE_BADGES: Map<number, 'new' | 'popular'> = new Map([
  // These IDs should match actual recipe IDs when available
  // For demo purposes, we use placeholder IDs
  [1, 'new'],
  [2, 'popular'],
  [5, 'new'],
  [8, 'popular'],
  [12, 'new']
]);

// -----------------------------------------------------------------------------
// @MOCK: User Likes
// Used in: RecipeCard (heart icon state)
// Set of recipe IDs that the current user has liked
// In production, this would come from user's profile/preferences
// -----------------------------------------------------------------------------
export const MOCK_USER_LIKES: Set<number> = new Set([
  2, 5, 8, 15 // Recipe IDs the user has liked
]);

// -----------------------------------------------------------------------------
// @MOCK: Current User
// Used in: NavbarComponent (avatar), various auth checks
// Represents the currently logged in user for demo purposes
// -----------------------------------------------------------------------------
export const MOCK_CURRENT_USER = {
  id: 1,
  name: 'Stefano',
  email: 'stefano@example.com',
  avatar: 'https://images.unsplash.com/photo-1535713875002-d1d0cf377fde?w=100&h=100&fit=crop&crop=face'
};

// -----------------------------------------------------------------------------
// Helper Functions
// -----------------------------------------------------------------------------

/**
 * @MOCK: Check if a recipe has a badge
 * @param recipeId - The ID of the recipe to check
 * @returns The badge type or null if no badge
 */
export function getRecipeBadge(recipeId: number): 'new' | 'popular' | null {
  return MOCK_RECIPE_BADGES.get(recipeId) || null;
}

/**
 * @MOCK: Check if the current user has liked a recipe
 * @param recipeId - The ID of the recipe to check
 * @returns True if the user has liked the recipe
 */
export function isRecipeLiked(recipeId: number): boolean {
  return MOCK_USER_LIKES.has(recipeId);
}

/**
 * @MOCK: Toggle like state for a recipe
 * @param recipeId - The ID of the recipe to toggle
 * @returns The new like state
 */
export function toggleRecipeLike(recipeId: number): boolean {
  if (MOCK_USER_LIKES.has(recipeId)) {
    MOCK_USER_LIKES.delete(recipeId);
    return false;
  } else {
    MOCK_USER_LIKES.add(recipeId);
    return true;
  }
}
