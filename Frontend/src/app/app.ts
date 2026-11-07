import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PetApi } from './pet-api';
import { Pet } from './pet';

@Component({
  imports: [RouterOutlet],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App implements OnInit {
  private readonly petApi = inject(PetApi);

  protected readonly pets = signal<Pet[]>([]);

  ngOnInit() {
    this.petApi.getPets().subscribe((pets) => this.pets.set(pets));
  }
}
