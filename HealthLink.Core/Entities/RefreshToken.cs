using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthLink.Core.Entities
{
    public class RefreshToken
    {
        public readonly Guid id;
        public string refreshToken { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string IpAddress { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual User User { get; set; }
        public RefreshToken(Guid id, string refreshToken, DateTime ExpiryDate, string ipAddress)
        {
            this.id = id;
            this.refreshToken = refreshToken;
            this.ExpiryDate = ExpiryDate;
            IpAddress = ipAddress;
        }
    }
}
