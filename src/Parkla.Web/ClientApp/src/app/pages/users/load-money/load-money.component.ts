import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { RouteUrl } from '@app/core/utils/route.util';

@Component({
  selector: 'app-load-money',
  templateUrl: './load-money.component.html',
  styleUrls: ['./load-money.component.scss']
})
export class LoadMoneyComponent implements OnInit {

  cardNumber: string = "";
  expireMonth: number | null = null;
  expireYear: number | null = null;
  nameOnCard: string = "";
  cvc: number | null = null;

  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  goBack() {
    this.router.navigate([".."]);
  }

  pay(form: NgForm) {
    console.log("submitted");
    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }
  }
}
