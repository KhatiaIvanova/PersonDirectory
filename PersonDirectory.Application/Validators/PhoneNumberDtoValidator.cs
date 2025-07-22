using FluentValidation;
using PersonDirectory.Application.DTOs;
using PersonDirectory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonDirectory.Application.Validators
{
    public class PhoneNumberDtoValidator : AbstractValidator<PhoneNumberDto>
    {
        public PhoneNumberDtoValidator()
        {
            RuleFor(x => x.Type)
                .Must(t => new[] { PhoneType.მობილური, PhoneType.სახლი, PhoneType.ოფისი }.Contains(t));

            RuleFor(x => x.Number)
                .NotEmpty()
                .Length(4, 50);
        }
    }
}
