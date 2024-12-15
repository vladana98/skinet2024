import {
  Directive,
  effect,
  inject,
  OnInit,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';
import { AccountService } from '../../core/services/account.service';

@Directive({
  selector: '[appIsAdmin]',
  standalone: true,
})
export class IsAdminDirective {
  private accounteService = inject(AccountService);
  private viewContainerRef = inject(ViewContainerRef);
  private templateRef = inject(TemplateRef);

  constructor() {
    effect(() => {
      if (this.accounteService.isAdmin()) {
        this.viewContainerRef.createEmbeddedView(this.templateRef);
      } else {
        this.viewContainerRef.clear();
      }
    });
  }
}
