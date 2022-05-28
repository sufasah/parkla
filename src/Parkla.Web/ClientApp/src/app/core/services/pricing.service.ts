import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { apiPricings } from '../constants/http';
import { Pricing } from '../models/pricing';

@Injectable({
  providedIn: 'root'
})
export class PricingService {

  constructor(private httpClient: HttpClient) { }
}
