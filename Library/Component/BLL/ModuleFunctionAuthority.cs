using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using log4net;
using Oracle.ManagedDataAccess.Client;

using Library.Component.DAL;

namespace Library.Component.BLL
{
    public class ModuleFormFunction
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ModuleFormFunction));
    }

    public class ModuleFormFunctionCollection : List<ModuleFormFunction>
    {
        /// <summary>
        /// 預設建構子。
        /// </summary>
        public ModuleFormFunctionCollection()
        {

        }
    }
}