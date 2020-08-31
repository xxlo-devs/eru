using System;
using System.Collections.Generic;
using System.Text;
using eru.Infrastructure.Translation;
using FluentAssertions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace eru.Infrastructure.Tests.Translation
{
    public class TranslationTests
    {
        [Fact]
        public async void CanRetrieveTranslations()
        {
            var options = Options.Create(new LocalizationOptions());
            var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
            var localizer = new StringLocalizer<TranslationTests>(factory);

            var translator = new Translator<TranslationTests>(localizer);

            var en = await translator.TranslateString("test-value", "en");
            var pl = await translator.TranslateString("test-value", "pl");

            en.Should().Be("A test value!");
            pl.Should().Be("Testowa wartosc!");
        }
    }
}
