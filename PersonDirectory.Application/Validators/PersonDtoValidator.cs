using FluentValidation;
using PersonDirectory.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonDirectory.Application.Validators;

public static class PersonValidatorHelper
{
    public static void ApplyCommonRules<T>(AbstractValidator<T> validator) where T : PersonDto
    {
        validator.RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(2, 50)
            .Matches("^([ა-ჰ]+|[a-zA-Z]+)$")
            .WithMessage("სახელი უნდა შეიცავდეს მხოლოდ ქართულ ან ლათინურ ასოებს, ერთდროულად არა.");

        validator.RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(2, 50)
            .Matches("^([ა-ჰ]+|[a-zA-Z]+)$")
            .WithMessage("გვარი უნდა შეიცავდეს მხოლოდ ქართულ ან ლათინურ ასოებს, ერთდროულად არა.");

        validator.RuleFor(x => x.Gender)
            .Must(g => g == "კაცი" || g == "ქალი");

        validator.RuleFor(x => x.PersonalNumber)
            .NotEmpty()
            .Length(11)
            .Matches("^[0-9]{11}$");

        validator.RuleFor(x => x.DateOfBirth)
            .Must(d => d <= DateTime.Today.AddYears(-18))
            .WithMessage("უნდა იყოს მინიმუმ 18 წლის.");

        validator.RuleForEach(x => x.PhoneNumbers)
            .SetValidator(new PhoneNumberDtoValidator());
    }
}

public class PersonCreateDtoValidator : AbstractValidator<PersonCreateDto>
{
    public PersonCreateDtoValidator()
    {
        PersonValidatorHelper.ApplyCommonRules(this);
    }
}

public class PersonUpdateDtoValidator : AbstractValidator<PersonUpdateDto>
{
    public PersonUpdateDtoValidator()
    {
        PersonValidatorHelper.ApplyCommonRules(this);
    }
}
