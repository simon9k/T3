using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace T3.Areas.Identity.Data
{
    public class AppUserManager<TUser> : UserManager<TUser>
        where TUser : AppUser
    {

        //For MutiTenant
        //customizing & Extending UserManager to MutiTenant Application
        //   * in startup.cs Services.AddIdentity...AddUserManager<>(), to register AppUserManager in the DI(Dependency Injection)
        //是否有简单的办法（而不是更改每一个UserManager的相关函数，加入Tenant限制？
        //  * 有，即采用global filter. but 注意filter应该用一个函数来获取当前TenantId，这样TenantId改变了，这个filter依然有效，过滤
        //  * 条件会随着TenantId改变而改变，而且，global filter是在DBContext.OnModelCreate()是设置并建立，而且由于EF的cache
        //  * 机制，OnModelCreate()是不会每次DBContext生成时就运行的。
        //global filter 带来的问题：登录时有时不需要过滤（这样会导致其它Tenant的无法登录）
        //  * 可设计一个Expression，返回bool，来实现可调节是否生效的filter
        //  * 解决，详见filter设置。

        public Guid TenantId { get; set; }

        public AppUserManager(IUserStore<TUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<TUser> passwordHasher, IEnumerable<IUserValidator<TUser>> userValidators, IEnumerable<IPasswordValidator<TUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<AppUserManager<TUser>> logger)
           : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {

        }

        //
        // 摘要:
        //     Returns an IQueryable of users if the store is an IQueryableUserStore
        public override IQueryable<TUser> Users
        {
            //get { return base.Users.Where(s => s.TenantId == this.TenantId); }
            get { return base.Users; }
        }

        //Instructor, same for Manager/Parent/asisitant/...
        public async Task<IList<TUser>> GetInstructorsAsync()
        {
            //get { return base.Users.Where(s => s.TenantId == this.TenantId); }
             return await base.GetUsersInRoleAsync("Instructor"); 
        }
        public async Task<IList<TUser>> GetParentsAsync()
        {
            //get { return base.Users.Where(s => s.TenantId == this.TenantId); }
            return await base.GetUsersInRoleAsync("Parent");
        }


        public override async Task<IdentityResult> CreateAsync(TUser user)
        {
            user.TenantId = this.TenantId;
            string claimType = "TenantId";
            string claimValue = user.TenantId.ToString();

            Claim claim = new Claim(claimType, claimValue, ClaimValueTypes.String);

            IdentityResult result = await base.CreateAsync(user);
            if (result.Succeeded)
                return await base.AddClaimAsync(user, claim);
            else
                return result;
            //return base.CreateAsync(user);
        }
        public override async Task<IdentityResult> CreateAsync(TUser user, string password)
        {
            user.TenantId = this.TenantId;
            string claimType = "TenantId";
            string claimValue = user.TenantId.ToString();

            Claim claim = new Claim(claimType, claimValue, ClaimValueTypes.String);

            IdentityResult result = await base.CreateAsync(user, password);
            if (result.Succeeded)
                return await base.AddClaimAsync(user, claim);
            else
                return result;
            //return base.CreateAsync(user);
        }

        public override Task<IdentityResult> DeleteAsync(TUser user)
        {

            return base.DeleteAsync(user);
        }

        public override Task<IdentityResult> UpdateAsync(TUser user)
        {
            return base.UpdateAsync(user);
        }

    }
}
