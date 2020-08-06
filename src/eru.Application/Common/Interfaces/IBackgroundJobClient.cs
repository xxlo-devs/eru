using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace eru.Application.Common.Interfaces
{
    public interface IBackgroundJobClient
    {
        string Enqueue(Expression<Func<Task>> methodToCall);
    }
}