using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Core
{
    public class Result<TSuccess, TError>
    {
        TSuccess? _success;
        TError? _error;

        public Result(TSuccess success)
        {
            _success = success;
        }

        public Result(TError? error)
        {
            _error = error;
        }

        public TSuccess? Success { get => _success; }
        public TError? Error { get => _error; }
    }
}
