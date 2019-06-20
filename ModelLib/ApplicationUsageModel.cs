using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib
{
    
    public class ApplicationUsageModel
    {
        public int ComputerID { get; set; }
        public int UserID { get; set; }
        public int ApplicationID { get; set; }
        public ComputerType ComputerType { get; set; }
    }

    public enum ComputerType
    {
        DESKTOP,
        LAPTOP
    }

}
