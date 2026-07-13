export type ReservationStatus = 'Pending' | 'Confirmed' | 'Cancelled';

export interface Reservation {
  id: string;
  userId: string;
  campingId: string;
  checkIn: string;
  checkOut: string;
  nights: number;
  totalPrice: number;
  status: ReservationStatus;
  createdOn: string;
}

export interface CreateReservationRequest {
  campingId: string;
  checkIn: string;
  checkOut: string;
  totalPrice: number;
}
