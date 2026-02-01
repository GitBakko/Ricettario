import { Injectable } from '@angular/core';
import Swal, { SweetAlertIcon, SweetAlertResult } from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class DialogService {

  constructor() { }

  /**
   * Show a simple success notification (Toast style)
   */
  success(message: string, title: string = 'Successo') {
      return Swal.fire({
          title: title,
          text: message,
          icon: 'success',
          toast: true,
          position: 'top-end',
          showConfirmButton: false,
          timer: 3000,
          timerProgressBar: true
      });
  }

  /**
   * Show an error alert
   */
  error(message: string, title: string = 'Errore') {
      return Swal.fire({
          title: title,
          html: message,
          icon: 'error',
          confirmButtonText: 'Chiudi',
          confirmButtonColor: '#d33'
      });
  }

  /**
   * Show a warning alert
   */
  warning(message: string, title: string = 'Attenzione') {
      return Swal.fire({
          title: title,
          text: message,
          icon: 'warning',
          confirmButtonText: 'OK',
          confirmButtonColor: '#ffc107'
      });
  }

  /**
   * Show an info alert
   */
  info(message: string, title: string = 'Info') {
      return Swal.fire({
          title: title,
          text: message,
          icon: 'info',
          confirmButtonText: 'OK',
          confirmButtonColor: '#0dcaf0'
      });
  }

  /**
   * Show a confirmation dialog. Returns true if confirmed.
   */
  async confirm(message: string, title: string = 'Sei sicuro?', confirmText: string = 'Sì, procedi', cancelText: string = 'Annulla'): Promise<boolean> {
      const result = await Swal.fire({
          title: title,
          text: message,
          icon: 'question',
          showCancelButton: true,
          confirmButtonColor: '#3085d6',
          cancelButtonColor: '#d33',
          confirmButtonText: confirmText,
          cancelButtonText: cancelText
      });
      return result.isConfirmed;
  }

  /**
   * Show a prompt dialog expecting input. Returns the value or null if cancelled.
   */
  async prompt(message: string, title: string = 'Inserisci valore', inputType: 'text' | 'number' | 'email' | 'password' = 'text'): Promise<string | null> {
      const result = await Swal.fire({
          title: title,
          text: message,
          input: inputType,
          showCancelButton: true,
          confirmButtonText: 'Conferma',
          cancelButtonText: 'Annulla'
      });
      
      if (result.isConfirmed && result.value) {
          return result.value;
      }
      return null;
  }
}
