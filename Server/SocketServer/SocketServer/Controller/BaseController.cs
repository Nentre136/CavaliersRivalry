using SocketGameProtocol;
using SocketServer.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Controller
{
    abstract class BaseController
    {
        protected RequestCode requestCode = RequestCode.RequestNone;
        public RequestCode GetRequestCode { get { return requestCode; } }
    }
}
