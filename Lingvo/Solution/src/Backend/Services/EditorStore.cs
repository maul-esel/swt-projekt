using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lingvo.Backend.Services
{
	public class EditorStore : IUserStore<Editor>, IUserPasswordStore<Editor>
	{
		private readonly DatabaseService _db;
		public EditorStore(DatabaseService db)
		{
			_db = db;
		}

		private async Task<IdentityResult> Save(CancellationToken cancellationToken)
		{
			try
			{
				await _db.SaveChangesAsync(cancellationToken);
			}
			catch (Exception)
			{
				return IdentityResult.Failed();
			}

			return IdentityResult.Success;
		}

		Task<IdentityResult> IUserStore<Editor>.CreateAsync(Editor user, CancellationToken cancellationToken)
		{
			_db.Editors.Add(user);
			return Save(cancellationToken);
		}

		Task<IdentityResult> IUserStore<Editor>.DeleteAsync(Editor user, CancellationToken cancellationToken)
		{
			_db.Editors.Remove(user);
			return Save(cancellationToken);
		}

		Task<Editor> IUserStore<Editor>.FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			return _db.Editors.FindAsync(new[] { userId }, cancellationToken);
		}

		Task<Editor> IUserStore<Editor>.FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			return _db.Editors.FirstOrDefaultAsync(editor => editor.Name == normalizedUserName, cancellationToken);
		}

		Task<string> IUserStore<Editor>.GetNormalizedUserNameAsync(Editor user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.Name);
		}

		Task<string> IUserStore<Editor>.GetUserIdAsync(Editor user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.Name);
		}

		Task<string> IUserStore<Editor>.GetUserNameAsync(Editor user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.Name);
		}

		Task IUserStore<Editor>.SetNormalizedUserNameAsync(Editor user, string normalizedName, CancellationToken cancellationToken)
		{
			return ((IUserStore<Editor>)this).SetUserNameAsync(user, normalizedName, cancellationToken);
		}

		Task IUserStore<Editor>.SetUserNameAsync(Editor user, string userName, CancellationToken cancellationToken)
		{
			user.Name = userName;
			_db.Update(user);
			return Save(cancellationToken);
		}

		Task<IdentityResult> IUserStore<Editor>.UpdateAsync(Editor user, CancellationToken cancellationToken)
		{
			_db.Update(user);
			return Save(cancellationToken);
		}

		Task IUserPasswordStore<Editor>.SetPasswordHashAsync(Editor user, string passwordHash, CancellationToken cancellationToken)
		{
			user.PasswordHash = passwordHash;
			_db.Update(user);
			return Save(cancellationToken);
		}

		Task<string> IUserPasswordStore<Editor>.GetPasswordHashAsync(Editor user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.PasswordHash);
		}

		Task<bool> IUserPasswordStore<Editor>.HasPasswordAsync(Editor user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.PasswordHash != null);
		}

		void IDisposable.Dispose()
		{
			_db.Dispose();
		}
	}
}
