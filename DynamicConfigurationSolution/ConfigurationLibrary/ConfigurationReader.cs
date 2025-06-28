using ConfigurationLibrary.DBConnection;
using ConfigurationLibrary.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ConfigurationLibrary
{

    /// <summary>
    /// Dinamik konfigürasyon verilerini periyodik olarak storage'dan çeken ve cacheleyen sınıftır.
    /// </summary>
    public class ConfigurationReader
    {
        private readonly string _applicationName;
        private readonly string _connectionString;
        private readonly int _refreshIntervalMs;
        private readonly MemoryCache _cache = new(new MemoryCacheOptions());
        private Timer? _timer;

        /// <summary>
        /// ConfigurationReader sınıfı, uygulamaya özgü ayarları storage'dan okuyup cache'e alır.
        /// </summary>
        /// <param name="applicationName">Uygulama adı (örnek: "SERVICE-A")</param>
        /// <param name="connectionString">SQL Server bağlantı stringi</param>
        /// <param name="refreshIntervalMs">Kaç milisaniyede bir güncelleme yapılacağı</param>
        public ConfigurationReader(string applicationName, string connectionString, int refreshIntervalMs)
        {
            _applicationName = applicationName;
            _connectionString = connectionString;
            _refreshIntervalMs = refreshIntervalMs;

            LoadConfigurations(); // Başlangıç yüklemesi
            StartTimer();         // Periyodik güncellemeyi başlat
        }

        /// <summary>
        /// Timer başlatarak belirli aralıklarla storage'dan veri güncellemesi yapılmasını sağlar.
        /// </summary>
        private void StartTimer()
        {
            _timer = new Timer(_ => LoadConfigurations(), null, _refreshIntervalMs, _refreshIntervalMs);
        }

        /// <summary>
        /// Storage'dan ilgili uygulamaya ait aktif konfigürasyon kayıtlarını okur ve cache'e alır.
        /// Bağlantı hatası durumunda cache'teki veriler kullanılmaya devam eder.
        /// </summary>
        private void LoadConfigurations()
        {
            try
            {
                using var context = new ConfigurationDbContext(_connectionString);

                var configs = context.ConfigurationRecords
                    .Where(c => c.IsActive && c.ApplicationName == _applicationName)
                    .ToList();

                foreach (var config in configs)
                {
                    _cache.Set(config.Name, config);
                }
            }
            catch
            {
                // Hata durumunda cache'teki son başarılı veriler kullanılmaya devam eder.
            }
        }

        /// <summary>
        /// Verilen anahtar adına karşılık gelen konfigürasyon değerini istenilen tipte döner.
        /// </summary>
        /// <typeparam name="T">Dönüş tipi (string, int, bool, double vs.)</typeparam>
        /// <param name="key">Konfigürasyon anahtarı</param>
        /// <returns>İstenen tipte konfigürasyon değeri</returns>
        /// <exception cref="KeyNotFoundException">Anahtar bulunamazsa fırlatılır</exception>
        /// <exception cref="InvalidCastException">Tip dönüşümünde hata olursa fırlatılır</exception>
        public T GetValue<T>(string key)
        {
            if (_cache.TryGetValue(key, out ConfigurationItem? config))
            {
                try
                {
                    return (T)Convert.ChangeType(config.Value, typeof(T));
                }
                catch
                {
                    throw new InvalidCastException($"'{key}' değeri '{typeof(T)}' türüne dönüştürülemiyor.");
                }
            }

            throw new KeyNotFoundException($"'{key}' anahtarı bulunamadı.");
        }
    }
}
