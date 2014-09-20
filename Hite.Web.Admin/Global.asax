<%@ Application Language="C#" %>

<script runat="server">
    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup
        log4net.Config.XmlConfigurator.Configure();
    }
    void Application_Error(object sender, EventArgs e)
    {
        //发生系统级别未处理的异常时，调用Log4Net记录异常日志
        Exception ex = Server.GetLastError().GetBaseException();
        string msg = string.Format("{0} 发生异常:{1}\r\n{2}",
              Request.Url.LocalPath,
              ex.Message,
              ex.StackTrace
              );
        log4net.ILog Logger = log4net.LogManager.GetLogger("Global");

        Logger.Error(msg);
    }
</script>
