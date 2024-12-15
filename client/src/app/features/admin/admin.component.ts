import {
  AfterViewInit,
  Component,
  inject,
  OnInit,
  ViewChild,
} from '@angular/core';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { Order } from '../../shared/models/order';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { AdminService } from '../../core/services/admin.service';
import { OrderParams } from '../../shared/models/orderParams';
import { MatIcon } from '@angular/material/icon';
import { MatLabel, MatSelectChange, MatSelectModule } from '@angular/material/select';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatButton } from '@angular/material/button';
import { MatTabsModule } from '@angular/material/tabs';
import { RouterLink } from '@angular/router';
import { DialogService } from '../../core/services/dialog.service';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [
    MatTableModule,
    MatPaginatorModule,
    MatIcon,
    MatSelectModule,
    DatePipe,
    CurrencyPipe,
    MatLabel,
    MatTooltipModule,
    MatButton,
    MatTabsModule,
    RouterLink
    
  ],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.scss',
})
export class AdminComponent implements OnInit {
  displayedColumns: string[] = [
    'id',
    'buyerEmail',
    'orderDate',
    'total',
    'status',
    'action',
  ];
  dataSource = new MatTableDataSource<Order>([]);
  private adminService = inject(AdminService);
  orderParams = new OrderParams();
  totalItems = 0;
  statusOptions = [
    'All',
    'PaymentReceived',
    'PaymentMismatch',
    'Refunded',
    'Pending',
  ];
  private dialogService = inject(DialogService);


  ngOnInit(): void {
    this.loadOrders();
  }

  
  loadOrders() {
    this.adminService.getOrders(this.orderParams).subscribe({
      next: (response) => {
        if (response.data) {
          this.dataSource.data = response.data;

          this.totalItems = response.count;
        }
      },
    });
  }
  onpPageChange(event: PageEvent) {
    this.orderParams.pageNumber = event.pageIndex + 1;
    this.orderParams.pageSize = event.pageSize;
    this.loadOrders();
  }

  onFilterSelect(event: MatSelectChange) {
    this.orderParams.filter = event.value;
    this.orderParams.pageNumber = 1;
    this.loadOrders();
  }

  async openConfirmDialog(id: number){
    const confirmed = await this.dialogService.confirm(
      'Potvrda povrata',
      'Da li ste sigurni da želite izvršiti povrat nova?'
    )

    if (confirmed) this.refundOrder(id);
  }

  refundOrder(id: number){
    this.adminService.refundOrder(id).subscribe({
      next: order => {
        this.dataSource.data = this.dataSource.data.map(o => o.id === id ? order : o)
      }
    })
  }
}