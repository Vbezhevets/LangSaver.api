using FluentValidation;

public class WordQueryValidators : AbstractValidator <WordQueryRequest>
{
    public WordQueryValidator()
    {
        RuleFor(w =>w.UserId)
            .NotEmpty();
        RuleFor(w => w.Name)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(w => w.From)
            .NotEmpty()
            .Length(2);
        RuleFor(w => w.To)
            .NotEmpty()
            .Length(2);
        RuleFor(w => w.Category)
            .MaximumLength(30)
            .When(w => w.Category != null);

    }
}