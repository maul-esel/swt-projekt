using System;
namespace Lingvo.Backend
{
	/// <summary>
	/// An editor for the editing system.
	/// Contains user information necessary for login & logout.
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
		/// The hashed value is supplied by the Identity framework.
		/// </summary>
		/// <value>The password.</value>
		public String PasswordHash
		{
			get;
			set;
		}
	}
}
