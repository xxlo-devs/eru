using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using eru.Application.Common.Exceptions;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;

namespace eru.Infrastructure.XmlParsing
{
    public class SubstitutionsPlanXmlParser : ISubstitutionsPlanXmlParser
    {
        private readonly XmlParsingModelsMapper _mapper;

        public SubstitutionsPlanXmlParser(XmlParsingModelsMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<SubstitutionsPlan> Parse(Stream content)
        {
            var xml = (await GetString(content))
                .Replace("windows-1250", "utf-8");
            var serializer = new XmlSerializer(typeof(Models.Substitutions));
            var xmlReader = XmlReader.Create(ToStream(xml));
            if (serializer.CanDeserialize(xmlReader))
            {
                var result = serializer.Deserialize(xmlReader);
                if (result is Models.Substitutions substitutions)
                {
                    return _mapper.MapToSubstitutionsPlan(substitutions.Date);
                }
            }

            throw new ParsingException(typeof(Models.Substitutions));
        }

        private async Task<string> GetString(Stream stream) => await new StreamReader(stream).ReadToEndAsync();

        private Stream ToStream(string data) => new MemoryStream(Encoding.Default.GetBytes(data));
    }
}