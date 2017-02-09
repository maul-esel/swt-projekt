using System;
namespace Lingvo.Backend
{
	/// <summary>
	/// A Editor for the editing system.
	/// </summary>
	public class Editor
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public String Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the hashed password.
		/// </summary>
		/// <value>The password.</value>
		public String PasswordHash
		{
			get;
			set;
		}
	}
}
