import { Service, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Pet } from './pet';

// Every call to the backend lives in a service like this one.
@Service()
export class PetApi {
  private readonly http = inject(HttpClient);

  // "/api/..." is forwarded to the ASP.NET backend by proxy.conf.json
  getPets() {
    return this.http.get<Pet[]>('/api/pets');
  }
}
