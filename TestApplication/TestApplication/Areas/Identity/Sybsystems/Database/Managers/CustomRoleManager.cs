using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace TestApplication.Areas.Identity.Managers
{
    /// <summary>
    /// Переопределение ролей
    /// </summary>
    public class CustomIdentityRole : IdentityRole
    {

    }

    /// <summary>
    /// Переопределение менеждера ролей
    /// </summary>
    public class CustomRoleManager : RoleManager<CustomIdentityRole>, IDisposable
    {
        /// <inheritdoc/>
        public CustomRoleManager(IRoleStore<CustomIdentityRole> store, IEnumerable<IRoleValidator<CustomIdentityRole>> roleValidators, 
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<CustomIdentityRole>> logger)
            : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
        /// <inheritdoc/>
        public override ILogger Logger { get => base.Logger; set => base.Logger = value; }
        /// <inheritdoc/>
        public override IQueryable<CustomIdentityRole> Roles => base.Roles;
        /// <inheritdoc/>
        public override bool SupportsQueryableRoles => base.SupportsQueryableRoles;
        /// <inheritdoc/>
        public override bool SupportsRoleClaims => base.SupportsRoleClaims;
        /// <inheritdoc/>
        protected override CancellationToken CancellationToken => base.CancellationToken;
        /// <inheritdoc/>
        public override Task<IdentityResult> AddClaimAsync(CustomIdentityRole role, Claim claim)
        {
            return base.AddClaimAsync(role, claim);
        }
        /// <inheritdoc/>
        public override Task<IdentityResult> CreateAsync(CustomIdentityRole role)
        {
            return base.CreateAsync(role);
        }
        /// <inheritdoc/>
        public override Task<IdentityResult> DeleteAsync(CustomIdentityRole role)
        {
            return base.DeleteAsync(role);
        }
        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        /// <inheritdoc/>
        public override Task<CustomIdentityRole> FindByIdAsync(string roleId)
        {
            return base.FindByIdAsync(roleId);
        }
        /// <inheritdoc/>
        public override Task<CustomIdentityRole> FindByNameAsync(string roleName)
        {
            return base.FindByNameAsync(roleName);
        }
        /// <inheritdoc/>
        public override Task<IList<Claim>> GetClaimsAsync(CustomIdentityRole role)
        {
            return base.GetClaimsAsync(role);
        }
        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <inheritdoc/>
        public override Task<string> GetRoleIdAsync(CustomIdentityRole role)
        {
            return base.GetRoleIdAsync(role);
        }
        /// <inheritdoc/>
        public override Task<string> GetRoleNameAsync(CustomIdentityRole role)
        {
            return base.GetRoleNameAsync(role);
        }
        /// <inheritdoc/>
        public override string NormalizeKey(string key)
        {
            return base.NormalizeKey(key);
        }
        /// <inheritdoc/>
        public override Task<IdentityResult> RemoveClaimAsync(CustomIdentityRole role, Claim claim)
        {
            return base.RemoveClaimAsync(role, claim);
        }
        /// <inheritdoc/>
        public override Task<bool> RoleExistsAsync(string roleName)
        {
            return base.RoleExistsAsync(roleName);
        }
        /// <inheritdoc/>
        public override Task<IdentityResult> SetRoleNameAsync(CustomIdentityRole role, string name)
        {
            return base.SetRoleNameAsync(role, name);
        }
        /// <inheritdoc/>
        public override string ToString()
        {
            return base.ToString();
        }
        /// <inheritdoc/>
        public override Task<IdentityResult> UpdateAsync(CustomIdentityRole role)
        {
            return base.UpdateAsync(role);
        }
        /// <inheritdoc/>
        public override Task UpdateNormalizedRoleNameAsync(CustomIdentityRole role)
        {
            return base.UpdateNormalizedRoleNameAsync(role);
        }
        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        /// <inheritdoc/>
        protected override Task<IdentityResult> UpdateRoleAsync(CustomIdentityRole role)
        {
            return base.UpdateRoleAsync(role);
        }
        /// <inheritdoc/>
        protected override Task<IdentityResult> ValidateRoleAsync(CustomIdentityRole role)
        {
            return base.ValidateRoleAsync(role);
        }
    }
}
