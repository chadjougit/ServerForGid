using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PWappServer
{
    
   


    public static class StaticClass
    {
        


        public static List<string> Sessions = new List<string>();

     public  static  void Session_Start(string test)
        {
            try { Sessions.Add(test); }
            catch (Exception ex)
            { }
          
        }


    }
}
