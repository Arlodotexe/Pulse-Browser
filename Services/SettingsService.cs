using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;

namespace Pulse_Browser.Services
{
    public enum SettingKeys
    {
        PrefferedTheme
    }

    public static class SettingsService
    {
        private static readonly IPropertySet LocalSettings = ApplicationData.Current.LocalSettings.Values;

        public static T GetLocal<T>(SettingKeys key) => LocalSettings.ContainsKey(key.ToString()) ? JsonConvert.DeserializeObject<T>(LocalSettings[key.ToString()] as string) : default;
        public static void SetLocal<T>(SettingKeys key, T value)
        {
            string serializedValue = JsonConvert.SerializeObject(value);

            if (!LocalSettings.ContainsKey(key.ToString())) LocalSettings.Add(key.ToString(), serializedValue);
            else LocalSettings[key.ToString()] = serializedValue;
        }
    }
}
