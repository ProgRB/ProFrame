using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProFrame
{
    public static class ActivatorHelper
    {
        public static T CreateAndUnwrap<T>(string typeName)
        {
            return (T)(Activator.CreateInstance(ProviderSetting.ConfigurationProvider.AssemblyName, ProviderSetting.ConfigurationProvider.AssemblyNameSpace + "." + typeName).Unwrap());
        }
    }
}
