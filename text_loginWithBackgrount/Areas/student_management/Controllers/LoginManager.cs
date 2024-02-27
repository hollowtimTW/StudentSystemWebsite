using System.Security.Cryptography;
using System.Text;

namespace Class_system_Backstage_pj.Areas.student_management.Controllers
{
    public class LoginManager
    {



        private static LoginManager instance;

        private LoginManager() { }
        public static LoginManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LoginManager();
                }
                return instance;
            }
        }
    }
}
