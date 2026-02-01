import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-coming-soon',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslateModule],
  template: `
    <div class="coming-soon-container">
      <div class="coming-soon-card">
        <!-- Decorative Icon -->
        <div class="coming-soon-icon">
          <i [class]="'fas ' + icon()"></i>
        </div>
        
        <!-- Title -->
        <h1 class="coming-soon-title">{{ 'COMING_SOON.TITLE' | translate }}</h1>
        
        <!-- Page Name -->
        <h2 class="coming-soon-page">{{ pageName() }}</h2>
        
        <!-- Message -->
        <p class="coming-soon-message">{{ 'COMING_SOON.MESSAGE' | translate }}</p>
        
        <!-- Decorative bread illustration -->
        <div class="coming-soon-decoration">
          <i class="fas fa-bread-slice"></i>
          <i class="fas fa-croissant"></i>
          <i class="fas fa-wheat-awn"></i>
        </div>
        
        <!-- Back button -->
        <a routerLink="/" class="btn btn-accent rounded-pill px-4 mt-4">
          <i class="fas fa-home me-2"></i>
          {{ 'COMING_SOON.BACK_HOME' | translate }}
        </a>
      </div>
    </div>
  `,
  styles: [`
    .coming-soon-container {
      display: flex;
      align-items: center;
      justify-content: center;
      min-height: calc(100vh - 200px);
      padding: var(--kh-space-lg);
    }
    
    .coming-soon-card {
      text-align: center;
      max-width: 480px;
      padding: var(--kh-space-3xl);
      background: var(--kh-surface);
      border-radius: var(--kh-radius-xl);
      box-shadow: var(--kh-shadow-lg);
    }
    
    .coming-soon-icon {
      width: 100px;
      height: 100px;
      display: flex;
      align-items: center;
      justify-content: center;
      margin: 0 auto var(--kh-space-lg);
      border-radius: 50%;
      background: var(--kh-gradient-warm);
      
      i {
        font-size: 2.5rem;
        color: var(--kh-primary);
      }
    }
    
    .coming-soon-title {
      font-family: var(--kh-font-display);
      font-size: 2rem;
      font-weight: 700;
      color: var(--kh-accent);
      margin-bottom: var(--kh-space-xs);
    }
    
    .coming-soon-page {
      font-family: var(--kh-font-display);
      font-size: 1.5rem;
      font-weight: 600;
      color: var(--kh-text-primary);
      margin-bottom: var(--kh-space-md);
    }
    
    .coming-soon-message {
      color: var(--kh-text-muted);
      font-size: 1rem;
      line-height: 1.6;
      margin-bottom: var(--kh-space-lg);
    }
    
    .coming-soon-decoration {
      display: flex;
      justify-content: center;
      gap: var(--kh-space-lg);
      padding: var(--kh-space-md);
      
      i {
        font-size: 1.5rem;
        color: var(--kh-primary);
        opacity: 0.4;
        animation: float 3s ease-in-out infinite;
        
        &:nth-child(2) {
          animation-delay: 0.5s;
        }
        
        &:nth-child(3) {
          animation-delay: 1s;
        }
      }
    }
    
    @keyframes float {
      0%, 100% {
        transform: translateY(0);
      }
      50% {
        transform: translateY(-8px);
      }
    }
  `]
})
export class ComingSoonComponent {
  icon = input<string>('fa-clock');
  pageName = input<string>('');
}
