using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib
{
    public class ApplicationUsageAnalysisModel
    {
        public int ApplicationID { get; set; }

        // Number of users using the application 
        public int Users { get; set; }

        // Number of Desktops using the application
        public int Desktops { get; set; }

        // Number of Laptops using the application
        public int Laptops { get; set; }

        // Required number of application licences
        public int RequiredCopies { set; get; }
    }
}
