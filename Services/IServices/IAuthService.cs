using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintProxy.Services.IServices
{
    public interface IAuthService
    {
        Task<string> LoginAsync();
    }

}
