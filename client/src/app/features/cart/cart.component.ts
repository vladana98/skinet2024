import { Component, inject } from '@angular/core';
import { CartService } from '../../core/services/cart.service';
import { CarItemComponent } from "./cart-item/cart-item.component";
import { OrderSummaryComponent } from '../../shared/comonents/order-summary/order-summary.component';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CartComponent, CarItemComponent,OrderSummaryComponent],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss'
})
export class CartComponent {
  cartServices = inject(CartService);

  

}
