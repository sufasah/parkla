import { HttpErrorResponse } from '@angular/common/http';
import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '@app/core/services/auth.service';
import { RouteUrl } from '@app/core/utils/route';
import { selectAuthState } from '@app/store/auth/auth.selectors';
import { Store } from '@ngrx/store';
import { Message, MessageService } from 'primeng/api';
import { Subscription } from 'rxjs/internal/Subscription';

@Component({
  selector: 'app-verify',
  templateUrl: './verify.component.html',
  styleUrls: ['./verify.component.scss']
})
export class VerifyComponent implements OnInit, OnDestroy {

  @Output()
  onCancel = new EventEmitter();

  @Input()
  loading = false;

  @Input()
  username = "";

  @Input()
  password = "";

  @Output()
  onLogin = new EventEmitter<{successful: boolean, error: string | null}>();

  verifCode = "";
  verifying = false;

  private authStateSubscription?: Subscription;

  constructor(
    private authService: AuthService,
    private messageService: MessageService) { }

  ngOnInit(): void {
  }

  verify(form:NgForm) {
    if(this.verifying) return;
    if(form.invalid){
      var keys = Object.keys(form.controls);
      keys.forEach(e => {
        form.controls[e].markAsDirty()
      });
      return;
    }

    this.verifying = true;

    this.authService.verify(this.username, this.verifCode)
      .subscribe({
        next: () => {
          this.messageService.add({
            key:"veriftoast",
            life:1500,
            severity: 'success',
            summary: 'Verification',
            detail: 'Email is verified',
            icon:"pi-lock-open"
          });
          this.authService.login(this.username, this.password).subscribe({
            next: () => {
              this.verifying = false;
              this.onLogin.emit({successful: true, error: null});
            },
            error: (err: HttpErrorResponse) => {
              this.verifying = false;
              this.onLogin.emit({successful: false, error: err.error.message});
            }
          });
        },
        error: (err: HttpErrorResponse) => {
          this.messageService.add({
            key: "veriftoast",
            life:5000,
            severity:"error",
            summary: "Verification",
            detail: err.error.message,
            icon: "pi-lock",
          })
          this.verifying = false;
        }
      });
  }

  CancelVerify() {
    this.onCancel.emit();
  }

  ngOnDestroy(): void {
    this.authStateSubscription?.unsubscribe();
  }

}
