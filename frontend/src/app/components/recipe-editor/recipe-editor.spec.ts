import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecipeEditorComponent } from './recipe-editor';

describe('RecipeEditorComponent', () => {
  let component: RecipeEditorComponent;
  let fixture: ComponentFixture<RecipeEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RecipeEditor]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RecipeEditor);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
