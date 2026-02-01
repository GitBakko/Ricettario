import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { PhaseTypeService } from '../../services/phase-type.service';
import { PhaseType } from '../../models/phase-type';
import { DialogService } from '../../services/dialog.service';
import { IconPickerComponent, IconOption } from '../../shared/components/icon-picker/icon-picker';

@Component({
  selector: 'app-phase-type-manager',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, IconPickerComponent],
  templateUrl: './phase-type-manager.html',
  styleUrls: ['./phase-type-manager.scss']
})
export class PhaseTypeManagerComponent implements OnInit {
  private service = inject(PhaseTypeService);
  private dialog = inject(DialogService);

  phaseTypes = signal<PhaseType[]>([]);
  editingId = signal<number | null>(null);

  formModel: Partial<PhaseType> = {
    name: '',
    description: '',
    icon: 'fa-solid fa-circle',
    color: '#6c757d',
    isActiveWork: true,
    isSystemDefault: false
  };

  availableIcons: IconOption[] = [
      { label: 'Generico', value: 'fa-solid fa-circle' },
      { label: 'Impasto', value: 'fa-solid fa-compact-disc' },
      { label: 'Attesa/Tempo', value: 'fa-solid fa-hourglass-half' },
      { label: 'Fuoco/Cottura', value: 'fa-solid fa-fire' },
      { label: 'Cesto/Pre-impasto', value: 'fa-solid fa-basket-shopping' },
      { label: 'Pieghe/Strati', value: 'fa-solid fa-layer-group' },
      { label: 'Formatura/Scatola', value: 'fa-solid fa-box' },
      { label: 'Appretto/Rotazione', value: 'fa-solid fa-rotate-right' },
      { label: 'Autolisi/Goccia', value: 'fa-solid fa-droplet' },
      { label: 'Frigo/Freddo', value: 'fa-solid fa-snowflake' },
      { label: 'Termometro', value: 'fa-solid fa-temperature-half' },
      { label: 'Mano/Azione', value: 'fa-solid fa-hand' },
      { label: 'Riposo/Orologio', value: 'fa-solid fa-clock-rotate-left' },
      { label: 'Fermentazione', value: 'fa-solid fa-virus' },
      { label: 'Griglia/Staglio', value: 'fa-solid fa-border-all' }
  ];

  ngOnInit() {
    this.loadTypes();
  }

  loadTypes() {
    this.service.getPhaseTypes().subscribe({
      next: (data) => this.phaseTypes.set(data),
      error: (err) => this.dialog.error('Errore nel caricamento delle fasi')
    });
  }

  edit(type: PhaseType) {
    this.editingId.set(type.id);
    this.formModel = { ...type };
  }

  cancelEdit() {
    this.editingId.set(null);
    this.resetForm();
  }

  resetForm() {
    this.formModel = {
      name: '',
      description: '',
      icon: 'fa-solid fa-circle',
      color: '#6c757d',
      isActiveWork: true,
      isSystemDefault: false
    };
  }

  save() {
    if (!this.formModel.name) {
       this.dialog.error('Il nome è obbligatorio');
       return;
    }

    if (this.editingId()) {
       // Update
       const id = this.editingId()!;
       this.service.updatePhaseType(id, { ...this.formModel, id } as PhaseType).subscribe({
           next: () => {
               this.dialog.success('Fase aggiornata con successo');
               this.loadTypes();
               this.cancelEdit();
           },
           error: () => this.dialog.error('Errore durante l\'aggiornamento')
       });
    } else {
       // Create
       this.service.createPhaseType(this.formModel as PhaseType).subscribe({
           next: () => {
               this.dialog.success('Fase creata con successo');
               this.loadTypes();
               this.resetForm();
           },
           error: () => this.dialog.error('Errore durante la creazione')
       });
    }
  }

  delete(type: PhaseType) {
    this.dialog.confirm(`Vuoi davvero eliminare la fase "${type.name}"?`).then(confirmed => {
        if (confirmed) {
            this.service.deletePhaseType(type.id).subscribe({
                next: () => {
                    this.dialog.success('Fase eliminata');
                    this.loadTypes();
                },
                error: () => this.dialog.error('Impossibile eliminare questa fase (potrebbe essere in uso)')
            });
        }
    });
  }
}
