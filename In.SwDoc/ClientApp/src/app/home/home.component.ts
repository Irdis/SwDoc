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
  constructor(
    private http: HttpClient,
    private formBuilder: FormBuilder) {
    this.urlForm = this.formBuilder.group({
      url:'https://petstore.swagger.io/v2/swagger.json'
    });
  }

  onSubmit() {
    this.urlForm.disable();
    this.urlDocId = null;
    this.urlDocInProgress = true;
    const headers = new HttpHeaders().set('Content-Type', 'application/json; charset=utf-8');
    this.http.post<GenerationResult>("api/sw-generator/url",
      JSON.stringify({
        url: this.urlForm.value.url
      }), { headers: headers }).subscribe(result => {
        this.urlDocId = result.id;
        this.urlDocInProgress = false;
        this.urlForm.enable();
    });
    
  }
}
