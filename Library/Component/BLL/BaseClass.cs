using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Hosting;

using log4net;

using CockpitTripTradeAdmin.App_Code.Enums;

namespace CockpitTripTradeAdmin.App_Code.BLL
{
    public abstract class BaseClass
    {
        public BaseClass()
        {

        }

        private void Load()
        {

        }

        public abstract bool Save(PageMode.PageModeEnum pm);

    }
}