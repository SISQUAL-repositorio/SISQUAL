using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using log4net.Appender;
using log4net.Config;

namespace Log4net
{
    [Serializable]
    public class Log4netClass
    {

        private static Log4netClass instance;
        public static Log4netClass Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Log4netClass();
                }

                return instance;
            }
        }

        private ILog logger = null;
        public ILog Logger
        {
            get
            {
                return this.logger;
            }
        }

        [ImportMany(typeof(ILoginManager))]
        public IEnumerable<ILoginManager> LoginManagers
        {
            get; set;
        }

        private void InitializeLogger()
        {
            try
            {
                XmlConfigurator.Configure();
            }
            catch (Exception)
            {
                // ...
            }

            //Refresh app user using login information
            LogicalThreadContext.Properties["spuser"] = "SISQUAL";

            this.logger = LogManager.GetLogger("sisqualponto.standard_log");
        }

        private void DoSometing()
        {
            try
            {
                
			
			string errorMessage = "error";

            this.Logger.Info(string.Format("Info message for log [{0}]", errorMessage));
            }
            catch (Exception ex)
            {
                this.Logger.Error("DoSomething", ex);
            }

        }
    }
}
