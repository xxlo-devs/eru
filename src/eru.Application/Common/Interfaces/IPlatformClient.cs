using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Domain.Entity;

namespace eru.Application.Common.Interfaces
{
    public interface IPlatformClient
    {
        public string PlatformId { get; }
        public Task SendMessage(string id, string content);
        public Task SendMessage(string id, IEnumerable<Substitution> substitutions);
    }
}