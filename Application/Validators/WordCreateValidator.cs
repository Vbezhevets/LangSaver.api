using FluentValidation;
public class WordCreateValidator : AbstractValidator <WordCreateRequest>  
{
    public WordCreateValidator()
    {
        
        RuleFor(w => w.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50);
        RuleFor(w => w.From)
            .NotEmpty().WithMessage("Languege from is required.")
            .Length(2);
        RuleFor(w => w.To)
            .NotEmpty().WithMessage("Languege to is required.")
        RuleFor(w => w.Category)
            .MaximumLength(50);

    }
}

/*

⸻

🟩 Готов сделать то же самое для:
	•	PUT
	•	PATCH
	•	DELETE
	•	GET
	•	Переводчика (интеграция с API)
	•	User authentication (лучше JWT или Google login)
	•	Pagination & Filtering
	•	Logging
	•	Repository Layer
	•	Services

С чего хочешь продолжить?

*/