using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace eru.Application.Common.Interfaces
{
    public interface IBackgroundExecutor
    {
        public string Enqueue(Expression<Func<Task>> method);
    }
}