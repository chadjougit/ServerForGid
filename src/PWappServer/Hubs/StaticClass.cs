using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWappServer
{
    
   


    public static class WebSocketSessions
    {
        



        public static List<ConnectionInfo> Sessions = new List<ConnectionInfo>();

     public  static  void Session_Start(ConnectionInfo ConnectionInfo)
        {
            try { Sessions.Add(ConnectionInfo); }
            catch (Exception ex)
            { }
          
        }


    }

    public class ConnectionInfo
    {
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
    }
}
