using FluentValidation;

public class WordUpdateValidator : AbstactValidator<WordUpdateRequest>
{
    public WordUpdateValidator ()
    {
                
        RuleFor(w => w.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50);
        RuleFor(w => w.Translation)
            .NotEmpty().WithMessage("Translation is required.")
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