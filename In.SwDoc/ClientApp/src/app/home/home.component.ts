import { Component } from '@angular/core';
import {
  HttpClient, HttpHeaders
} from '@angular/common/http';
import { FormBuilder } from '@angular/forms';
import { GenerationResult } from "./generation-result";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  urlForm;
  urlDocId;
  urlDocInProgress;
  urlDocError;

  specForm;
  specDocId;
  specDocError;
  specDocInProgress;
  constructor(
    private http: HttpClient,
    private formBuilder: FormBuilder) {
    this.urlForm = this.formBuilder.group({
      url:''
    });
    this.specForm = this.formBuilder.group({
      text: ''
    });
  }

  validateUrl() {
    this.urlDocError = null;
    if (this.urlForm.value.url == '') {
      this.urlDocError = "Url cannot be empty";
      return false;
    }
    return true;
  }

  validateSpec() {
    this.specDocError = null;
    if (this.specForm.value.text == '') {
      this.specDocError = "Json cannot be empty";
      return false;
    }
    return true;
  }

  onUrlSubmit() {
    if (!this.validateUrl()) {
      return;
    }
    this.urlForm.disable();
    this.urlDocId = null;
    this.urlDocError = null;
    this.urlDocInProgress = true;
    const headers = new HttpHeaders().set('Content-Type', 'application/json; charset=utf-8');
    this.http.post<GenerationResult>("api/sw-generator/url",
      JSON.stringify({
        url: this.urlForm.value.url
      }),
      { headers: headers }).subscribe(result => {
        if (result.error !== null) {
          this.urlDocError = this.getErrorMessage(result.error);
        } else {
          this.urlDocId = result.id;
        }
        this.urlDocInProgress = false;
        this.urlForm.enable();
      },
      error => {
        console.log('oops', error);
        this.urlDocError = "Ooops... Something went wrong";
        this.urlDocInProgress = false;
        this.urlForm.enable();
      });
  }

  getErrorMessage(errorCode) {
    if (errorCode === "WebException") {
      return "Unable to reach web site";
    } else if (errorCode === "GenerationError") {
      return "Unable to generate document";
    } else {
      return "Internal server error";
    }
  }

  onSpecSubmit() {
    if (!this.validateSpec()) {
      return;
    }
    this.specForm.disable();
    this.specDocId = null;
    this.specDocError = null;
    this.specDocInProgress = true;
    const headers = new HttpHeaders().set('Content-Type', 'application/json; charset=utf-8');
    this.http.post<GenerationResult>("api/sw-generator/spec",
      JSON.stringify({
        text: this.specForm.value.text
      }), { headers: headers }).subscribe(result => {
        if (result.error !== null) {
          this.specDocError = this.getErrorMessage(result.error);
        } else {
          this.specDocId = result.id;
        }
        this.specDocInProgress = false;
        this.specForm.enable();
    },
    error => {
      console.log('oops', error);
      this.specDocError = "Ooops... Something went wrong";
      this.specDocInProgress = false;
      this.specForm.enable();
    });
  }
}
