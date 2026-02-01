import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timeFormat',
  standalone: true
})
export class TimeFormatPipe implements PipeTransform {

  transform(minutes: number | null | undefined): string {
    if (!minutes || minutes < 0) {
      return '0m';
    }

    const hrs = Math.floor(minutes / 60);
    const mins = minutes % 60;

    if (hrs === 0) {
      return `${mins}m`;
    }

    if (mins === 0) {
      return `${hrs}h`;
    }

    return `${hrs}h ${mins}m`;
  }

}
