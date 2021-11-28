using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controllers
{
    interface Observer
    {
        void Update(string MessageType, int userId, int movieId, int submitScore);
    }
}
