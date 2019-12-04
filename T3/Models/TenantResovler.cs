using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace T3.Models
{
    //Get TenantId from httpContext Claims
    public interface ITenantResolver
    {
        Guid GetTenantId();
    }

    public class TenantResolver : ITenantResolver
    {
        //why httpContextAccessor? maybe HttpContext is not always available
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetTenantId()
        {
            //_httpContextAccessor.HttpContext.Items.TryGetValue("CURRENT_TENANT", out object currentTenant);
            //TenantId = (temp == null) ? Guid.Empty : new Guid(temp);

            var tenantId = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "TenantId")?.Value;
            if (string.IsNullOrEmpty(tenantId))
            {
                return Guid.Empty;
            }

            return new Guid(tenantId);
        }
    }
}
