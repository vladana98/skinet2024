import { Component, inject } from '@angular/core';
import { CartService } from '../../core/services/cart.service';
import { CarItemComponent } from "./cart-item/cart-item.component";
import { OrderSummaryComponent } from '../../shared/comonents/order-summary/order-summary.component';
import { EmptyStateComponent } from "../../shared/components/empty-state/empty-state.component";

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CartComponent, CarItemComponent, OrderSummaryComponent, EmptyStateComponent],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss'
})
export class CartComponent {
  cartServices = inject(CartService);

  

}
