import { Directive, ElementRef, Input, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormControl, NG_VALIDATORS, ValidationErrors, Validator } from '@angular/forms';

interface ValuesMatchElement extends HTMLElement {
  valuesMatchValidator:ValuesMatchValidator;
}

@Directive({
  selector: '[valuesMatch][ngModel],[valuesMatch][formControl]',
  providers: [
    {
      provide: NG_VALIDATORS,
      useExisting: ValuesMatchValidator,
      multi: true
    }
  ]
})
export class ValuesMatchValidator implements Validator, OnInit, OnDestroy {

  @Input("valuesMatch")
  key = "";

  @Input()
  formControl:FormControl | null = null;

  @Input()
  valuesMatchControl:FormControl | null = null;

  modelValue:any;

  private static elementsByKey: Map<string,Set<ElementRef<ValuesMatchElement>>> = new Map();

  get elementsByKey(){
    return ValuesMatchValidator.elementsByKey;
  }

  constructor(private ref:ElementRef<ValuesMatchElement>) {
    ref.nativeElement.valuesMatchValidator = this;
  }

  ngOnInit(): void {
    if(!this.elementsByKey.has(this.key))
      this.elementsByKey.set(this.key, new Set());

    this.elementsByKey.get(this.key)?.add(this.ref);


    this.formControl?.valueChanges.subscribe(newVal => {
      this.modelValue = newVal;
      this.validateOthers();
    });

    this.valuesMatchControl?.valueChanges.subscribe(newVal => {
      this.modelValue = newVal;
      this.validateOthers();
    });
  }

  ngOnDestroy(): void {

    if(this.elementsByKey.has(this.key)){
      this.elementsByKey.get(this.key)!.delete(this.ref);

      if(this.elementsByKey.get(this.key)!.size==0)
        this.elementsByKey.delete(this.key);
    }

  }

  validate(control: AbstractControl): ValidationErrors | null {
    if(!this.elementsByKey.has(this.key)) return null;

    let elementSet = this.elementsByKey.get(this.key)!;
    let result = [];

    for(let el of elementSet.values()){
      let elValidator = el.nativeElement.valuesMatchValidator;

      if(elValidator != this &&
        (!!elValidator.modelValue || !!control.value) &&
        elValidator.modelValue != control.value){

        result.push(el);
      }
    }

    return result.length > 0
      ? {valuesNotMatch:result}
      : null;
  }

  validateOthers(){
    this.elementsByKey.get(this.key)?.forEach(el => {
      let elValidator = el.nativeElement.valuesMatchValidator;
      if(elValidator != this){
        elValidator.formControl?.updateValueAndValidity({
          emitEvent:false
        });
        elValidator.valuesMatchControl?.updateValueAndValidity({
          emitEvent:false
        });
      }
    });
  }

}
