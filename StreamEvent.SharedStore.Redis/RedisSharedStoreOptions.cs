using System.ComponentModel.DataAnnotations;

namespace StreamEvent.SharedStore.Redis
{
    /// <summary>
    /// Shared state redis _options
    /// </summary>
    public class RedisSharedStoreOptions
    {
        [Required(AllowEmptyStrings =false)]
        public string RedisConnectionString { get; set; }
    }
}
