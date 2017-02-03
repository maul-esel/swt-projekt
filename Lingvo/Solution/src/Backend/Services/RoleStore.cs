using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

namespace Lingvo.Backend.Services
{
	// Currently unused - an implementation is nevertheless required by Identity.
	public class RoleStore : IRoleStore<object>
	{
		Task<IdentityResult> IRoleStore<object>.CreateAsync(object role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task<IdentityResult> IRoleStore<object>.DeleteAsync(object role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		void IDisposable.Dispose() { }

		Task<object> IRoleStore<object>.FindByIdAsync(string roleId, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task<object> IRoleStore<object>.FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task<string> IRoleStore<object>.GetNormalizedRoleNameAsync(object role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task<string> IRoleStore<object>.GetRoleIdAsync(object role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task<string> IRoleStore<object>.GetRoleNameAsync(object role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task IRoleStore<object>.SetNormalizedRoleNameAsync(object role, string normalizedName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task IRoleStore<object>.SetRoleNameAsync(object role, string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task<IdentityResult> IRoleStore<object>.UpdateAsync(object role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
