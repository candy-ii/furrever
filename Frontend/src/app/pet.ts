// Mirrors Backend/Models/Pet.cs. The API sends camelCase JSON.
export interface Pet {
  id: number;
  name: string;
  species: string;
}
