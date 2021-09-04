using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class MyReg
{
    #region Basic Functionality
    private static Microsoft.Win32.RegistryKey MyKey
    {
        get
        {
            // CreateSubKey => Creates a new subkey or opens an existing subkey for write access.
            return Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\LiveReader2");
        }
    }
    private static void SetMyValue(string name, object value)
    {
        var key = MyKey;
        key.SetValue(name, value);
        key.Close();
    }
    private static object GetMyValue(string name)
    {
        var key = MyKey;
        var value = key.GetValue(name);
        key.Close();
        return value;
    }
    #endregion
    #region Public Properties
    public static string ServerAddress
    {
        get
        {
            return TrimEndSlashes((string)GetMyValue("ServerAddress"));
        }
        set
        {
            SetMyValue("ServerAddress", TrimEndSlashes(value));
        }
    }
    #endregion
    private static string TrimEndSlashes(string text)
    {
        var result = text;
        while (result.EndsWith("/"))
        {
            result = result.Remove(result.Length - 1);
        }
        return result;
    }
}
