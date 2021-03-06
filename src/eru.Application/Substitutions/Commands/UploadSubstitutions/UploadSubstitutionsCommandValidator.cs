﻿using System;
using System.Net;
using eru.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace eru.Application.Substitutions.Commands.UploadSubstitutions
{
    public class UploadSubstitutionsCommandValidator : AbstractValidator<UploadSubstitutionsCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly IApplicationDbContext _context;

        public UploadSubstitutionsCommandValidator(IConfiguration configuration, IApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;

            RuleFor(x => x.IpAddress)
                .NotEmpty().WithMessage("IpAddress cannot be empty.")
                .Must(IsIpAddressValid).WithMessage("IpAddress must be a valid ip address.");
                
            RuleFor(x => x.Key)
                .NotEmpty().WithMessage("Key cannot be empty.")
                .Must(IsKeyValid).WithMessage("Key must be a correct key from configuration.");

            RuleFor(x => x.SubstitutionsDate)
                .NotEmpty().WithMessage("SubstitutionsDate cannot be empty.");

            RuleFor(x => x.UploadDateTime)
                .NotEmpty().WithMessage("UploadDateTime cannot be empty.")
                .Must(IsUploadDateTimeInThePast).WithMessage("UploadDateTime must be in the past.");
            
            RuleFor(x => x.Substitutions)
                .NotEmpty().WithMessage("Substitutions cannot be empty.");
        }

        private bool IsKeyValid(string key)
        {
            return _configuration.GetValue<string>("UploadKey") == key;
        }
        
        private bool IsIpAddressValid(string address)
            => IPAddress.TryParse(address, out _);

        private bool IsUploadDateTimeInThePast(DateTime uploadDateTime)
            => uploadDateTime.CompareTo(DateTime.Now) <= 0;
    }
}