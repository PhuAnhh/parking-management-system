import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import ValidateForm from '../../helpers/validateform';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-signup',
  standalone: false,
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss'
})
export class SignupComponent {
  type: string = "password"
  isText: boolean = false;
  eyeIcon: string = "fa-eye-slash"
  signUpForm!: FormGroup;
  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router){}

  ngOnInit(): void{
    this.signUpForm = this.fb.group({
      username:['', Validators.required],
      email:['', Validators.required],
      password:['', Validators.required]
    })
  }


  hideShowPass(){
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password"; 
  }

  onSignup(){
    if(this.signUpForm.valid){
      // perform logic for signup
      this.auth.signUp(this.signUpForm.value)
      .subscribe({
        next:(res=>{
          alert(res.message || "Đăng ký thành công");
          this.signUpForm.reset();
          this.router.navigate(['login']);
        }),
        error:(err=>{
          alert(err?.error.message)
        })
      })

      console.log(this.signUpForm.value)
    } else{
      ValidateForm.validateAllFormFileds(this.signUpForm)
      //logic for throwing error
    }
  }
  
}
