import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { MOCK_FEATURED_BAKERS, FeaturedBaker } from '../../core/data/mock-data';

@Component({
  selector: 'app-featured-baker',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './featured-baker.html',
  styleUrl: './featured-baker.scss'
})
export class FeaturedBakerComponent {
  // @MOCK: Featured bakers from mock data
  featuredBakers: FeaturedBaker[] = MOCK_FEATURED_BAKERS;

  // Generate star array for rating display
  getStars(rating: number): number[] {
    const fullStars = Math.floor(rating);
    return Array(5).fill(0).map((_, i) => i < fullStars ? 1 : 0);
  }

  // @MOCK: Connect button handler (no real functionality yet)
  onConnect(baker: FeaturedBaker): void {
    console.log('@MOCK: Connect clicked for', baker.name);
    // TODO: Implement actual connection logic
  }

  // @MOCK: Connect to first baker
  onConnectFirst(): void {
    if (this.featuredBakers.length > 0) {
      this.onConnect(this.featuredBakers[0]);
    }
  }
}
