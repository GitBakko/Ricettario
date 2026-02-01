import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { guestGuard } from './guards/guest.guard';

export const routes: Routes = [
    { 
        path: 'login', 
        loadComponent: () => import('./features/auth/login/login').then(m => m.LoginComponent),
        canActivate: [guestGuard]
    },
    { 
        path: 'register', 
        loadComponent: () => import('./features/auth/register/register').then(m => m.RegisterComponent),
        canActivate: [guestGuard]
    },
    { 
        path: '', 
        loadComponent: () => import('./components/recipe-list/recipe-list').then(m => m.RecipeListComponent),
        canActivate: [authGuard]
    },
    { 
        path: 'create', 
        loadComponent: () => import('./components/recipe-editor/recipe-editor').then(m => m.RecipeEditorComponent),
        canActivate: [authGuard]
    },
    { 
        path: 'phase-types', 
        loadComponent: () => import('./components/phase-type-manager/phase-type-manager').then(m => m.PhaseTypeManagerComponent),
        canActivate: [authGuard]
    },
    {
        path: 'categories',
        loadComponent: () => import('./components/category-manager/category-manager').then(m => m.CategoryManagerComponent),
        canActivate: [authGuard]
    },
    {
        path: 'tags',
        loadComponent: () => import('./components/tag-manager/tag-manager').then(m => m.TagManagerComponent),
        canActivate: [authGuard]
    },
    {
        path: 'ingredients',
        loadComponent: () => import('./pages/ingredient-archive-manager/ingredient-archive-manager').then(m => m.IngredientArchiveManagerComponent),
        canActivate: [authGuard]
    },
    { 
        path: 'editor/:id', 
        loadComponent: () => import('./components/recipe-editor/recipe-editor').then(m => m.RecipeEditorComponent),
        canActivate: [authGuard]
    },
    { 
        path: 'recipes/:id', 
        loadComponent: () => import('./components/recipe-detail/recipe-detail').then(m => m.RecipeDetailComponent),
        canActivate: [authGuard]
    },
    // Placeholder routes for Coming Soon pages
    {
        path: 'community',
        loadComponent: () => import('./pages/community/community-page.component').then(m => m.CommunityPageComponent),
        canActivate: [authGuard]
    },
    {
        path: 'profile',
        loadComponent: () => import('./pages/profile/profile-page.component').then(m => m.ProfilePageComponent),
        canActivate: [authGuard]
    }
];
