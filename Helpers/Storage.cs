using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Pulse_Browser.Helpers
{
    public static class Storage
    {
        private static readonly AsyncMutex SingleThread = new AsyncMutex();
        public static async Task<byte[]> GetBytesAsync(StorageFile file)
        {
            byte[] fileBytes = null;
            if (file == null) return null;
            using (var stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (var reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }
            return fileBytes;
        }
        public static async Task<byte[]> GetBytesAsync(BitmapImage imageSource)
        {
            if (imageSource.UriSource == default) return default;

            using (IRandomAccessStream inputStream = await RandomAccessStreamReference.CreateFromUri(imageSource.UriSource).OpenReadAsync())
            {
                var readStream = inputStream.AsStreamForRead();

                var byteArray = new byte[readStream.Length];
                await readStream.ReadAsync(byteArray, 0, byteArray.Length);
                return byteArray;
            }
        }
        private static readonly Dictionary<string, object> CacheMap = new Dictionary<string, object>();

        public static async Task<T> GetLocalFile<T>(string filename)
        {
            try
            {
                if (typeof(T) == typeof(string))
                {
                    StorageFile textFile = await ApplicationData.Current.LocalCacheFolder.GetFileAsync(filename);
                    if (await ApplicationData.Current.LocalCacheFolder.TryGetItemAsync(filename) is IStorageFile file)
                    {
                        Debug.WriteLine($"Retrieved {filename} successfully");
                        return (T)(object)await FileIO.ReadTextAsync(file);
                    }
                }

                if (typeof(T) == typeof(BitmapImage))
                {
                    StorageFile imageFile = await ApplicationData.Current.LocalCacheFolder.GetFileAsync(filename);
                    string fileContent = await FileIO.ReadTextAsync(imageFile);

                    if (string.IsNullOrEmpty(fileContent)) return default;

                    // Converting from base64 instead of screwing around with bitmap decoding and SoftwareBitmap
                    byte[] byteArray = Convert.FromBase64String(fileContent);

                    BitmapImage bitmap = new BitmapImage();
                    using (MemoryStream stream = new MemoryStream(byteArray))
                    {
                        await bitmap.SetSourceAsync(stream.AsRandomAccessStream());
                    }
                    Debug.WriteLine($"Retrieved {filename} successfully");
                    return (T)(object)bitmap;
                }

                return default;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error while getting local file: {filename} ({ex.Message})");
                return default;
            }

        }
        public static async void DeleteLocalFile(string filename)
        {
            try
            {
                using (await SingleThread.LockAsync())
                {
                    StorageFile file = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
                    await file?.DeleteAsync();
                }
            }
            catch
            {
                Debug.WriteLine($"Something went wrong while deleting a local file: { filename }");
            }
        }
        public static async Task StoreLocalFile<T>(string filename, T value)
        {
            try
            {
                if (value == null)
                {
                    DeleteLocalFile(filename);
                    return;
                }

                if (value is string)
                {
                    StorageFile file = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);

                    await FileIO.WriteTextAsync(file, value.ToString());
                    Debug.WriteLine($"Stored {filename} successfully");
                    return;
                }

                if (value is BitmapImage)
                {
                    StorageFile imageFile = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                    // Converting to base64 instead of screwing around with decoders and bitmaps
                    byte[] fileBytes = await GetBytesAsync(value as BitmapImage);
                    await FileIO.WriteTextAsync(imageFile, Convert.ToBase64String(fileBytes));
                    Debug.WriteLine($"Stored {filename} successfully");
                    return;
                }

                throw new NotImplementedException("Type not implemented");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Something went wrong while storing a local file: { filename }.\nError: {ex.Message}");
            }
        }

        public static async Task<T> GetLocalClass<T>(string name)
        {
            using (await SingleThread.LockAsync())
            {
                // Try to get the value from the runtime cache
                if (CacheMap.TryGetValue(name, out object value))
                {
                    if (value is T result && result != null) return result;
                }

                string fileContent = await Helpers.Storage.GetLocalFile<string>(name + ".json");
                if (string.IsNullOrEmpty(fileContent)) return default;

                T data = JsonConvert.DeserializeObject<T>(fileContent);
                CacheMap[name] = data; // Store in runtime cache for faster access in the future
                if (data is T) return data;

                return default(T);
            }
        }

        public static async Task StoreLocalClass(string name, object value)
        {
            using (await SingleThread.LockAsync())
            {
                string fileContent = JsonConvert.SerializeObject(value);
                CacheMap[name] = value;

                await StoreLocalFile(name + ".json", fileContent);
            }
        }
        public static T GetRoamingClass<T>(string name)
        {
            var list = ApplicationData.Current.RoamingSettings.Values.ToDictionary(x => x.Key, x => x.Value);

            var StoredData = list.FirstOrDefault(i => i.Key == name).Value;
            if (!(StoredData is string || StoredData is null)) throw new InvalidDataException("The retrieved class is not a string");

            if (string.IsNullOrEmpty(StoredData as string)) return default;

            T data = JsonConvert.DeserializeObject<T>(StoredData.ToString());

            if (data is T) return data;
            return default(T);
        }
        public static void SetRoamingClass(string name, object value)
        {
            string json = JsonConvert.SerializeObject(value);
            ApplicationData.Current.RoamingSettings.Values.Remove(name);

            ApplicationData.Current.RoamingSettings.Values[name] = json;
        }

        /// <summary>
        /// Clears all roaming data
        /// </summary>
        /// <returns>String with an error message, or null on success</returns>
        public static string ClearAllRoamingData()
        {
            try
            {
                ApplicationData.Current.RoamingSettings.Values.Clear();

                if (ApplicationData.Current.RoamingSettings.Values.ToArray().Length > 0)
                {
                    return ("Roaming values did not clear properly. Remaining values: " + string.Join(", ", ApplicationData.Current.RoamingSettings.Values.Keys.ToArray()));
                }
                else return null;

            }
            catch (Exception ex)
            {
                return (ex.Message);
            }
        }

        /// <summary>
        /// Clears all local data
        /// </summary>
        /// <returns>String with an error message, or null on success</returns>
        public static async Task<string> ClearAllLocalDataAsync()
        {
            // Clear local settings
            ApplicationData.Current.LocalSettings?.Values?.Clear();

            // Clear persistent storage
            var cacheFolder = await ApplicationData.Current.LocalCacheFolder.GetFilesAsync();

            foreach (IStorageFile file in cacheFolder)
            {
                if (file is null) continue;
                await file.DeleteAsync(StorageDeleteOption.Default);
            }


            var files = await ApplicationData.Current.LocalCacheFolder.GetFilesAsync();
            if (files.Any())
            {
                return ("Local data did not clear properly. Remaining files: " + string.Join(", ", files.Select(f => f.Name)));
            }

            else return null;
        }

        public static void ClearRoamingClass(string name)
        {
            ApplicationData.Current.RoamingSettings.Values[name] = null;
        }
    }
}
