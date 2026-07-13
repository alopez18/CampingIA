export interface Country {
  id: string;
  code: string;
  name: string;
}

export interface Province {
  id: string;
  code: string;
  name: string;
  countryId: string;
}
