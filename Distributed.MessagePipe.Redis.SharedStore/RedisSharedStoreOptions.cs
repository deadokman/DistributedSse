using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributed.MessagePipe.Redis.SharedStore
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
