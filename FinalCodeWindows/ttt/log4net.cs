using log4net;
using log4net.Appender;
using log4net.Config;

namespace XPTO
{
    [Serializable]
    public class Program
    {

        private static Program instance;
        public static Program Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Program();
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
			...
			
			string errorMessage = ...
			this.Logger.Info(string.Format("Info message for log [{0}]", errorMessage));
            }
            catch (Exception ex)
            {
                this.Logger.Error("DoSomething", ex);
            }

        }		
	}
}	