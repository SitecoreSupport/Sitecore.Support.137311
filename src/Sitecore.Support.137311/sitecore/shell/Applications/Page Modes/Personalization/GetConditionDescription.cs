using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.Rules;
using Sitecore.Shell.Web;
using Sitecore.Web;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Xml;
using System.Xml.Linq;

namespace Sitecore.Support.Shell.Applications.PageModes.Personalization
{
    public class GetConditionDescription : IHttpHandler, IReadOnlySessionState, IRequiresSessionState
    {
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (Settings.WebEdit.SimulatedCommandExecutionLatency > 0)
            {
                Thread.Sleep(Settings.WebEdit.SimulatedCommandExecutionLatency);
            }
            context.Response.Clear();
            context.Response.ContentType = "text/html";
            ShellPage.IsLoggedIn();
            string condition = WebUtil.GetFormValue("condition");
            Assert.IsNotNullOrEmpty(condition, "condition");
            XmlDocument conditionXml = JsonConvert.DeserializeXmlNode(condition);
            Assert.IsNotNull(conditionXml, "doc");
            XElement xElement = XElement.Parse(conditionXml.InnerXml);
            Assert.IsNotNull(xElement, "element");
            HtmlTextWriter htmlTextWriter = new HtmlTextWriter(new StringWriter());
            this.RenderRules(new object[]
            {
                htmlTextWriter,
                xElement
            });
            context.Response.Write(htmlTextWriter.InnerWriter.ToString());
        }

        protected void RenderRules(object[] parameters)
        {
            RulesRenderer obj = new RulesRenderer(string.Empty);
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
            typeof(RulesRenderer).GetMethod("RenderConditions", bindingAttr).Invoke(obj, parameters);
        }
    }
}
