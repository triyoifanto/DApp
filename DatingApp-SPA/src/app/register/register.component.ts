import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // the input properties thant sneded from parent component
  // @Input() valueFromHome: any;

  // the output properties that emit an event
  @Output() cancelRegister = new EventEmitter();
  model: any = {};
  registerForm: FormGroup;

  constructor(private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.registerForm = new FormGroup({
      username: new FormControl('', Validators.required),
      password: new FormControl('', [Validators.required, Validators.minLength(6)]),
      confirmPassword: new FormControl('', Validators.required)
    }, this.passwordMatchValidator);
  }

  passwordMatchValidator(form: FormGroup) {
    return form.get('password').value === form.get('confirmPassword').value ? null : {'mismatch': true};
  }

  register() {
    // this.authService.register(this.model)
    //   .subscribe(() => {
    //     this.alertify.success('register success');
    //   }, error => {
    //     this.alertify.error(error);
    //   });
    // console.log(this.model);
    console.log(this.registerForm.value);
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
