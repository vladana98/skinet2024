import { L } from '@angular/cdk/keycodes';
import { Injectable } from '@angular/core';
import { getTextOfJSDocComment } from 'typescript';

@Injectable({
  providedIn: 'root'
})
export class BusyService {

loading = false;
busyRequestCoint = 0;

busy(){
  this.busyRequestCoint++;
  this.loading = true;

}
idle(){
  this.busyRequestCoint--;
  if (this.busyRequestCoint <= 0){
    this.busyRequestCoint = 0;
    this.loading = false;
  }
}
}
