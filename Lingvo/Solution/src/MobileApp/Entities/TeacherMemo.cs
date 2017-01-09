﻿using Lingvo.Common.Entities;
using System;

namespace MobileApp.Entities
{
    /// <summary>
    /// Teacher memo: teacher can record a scentence or word the student cant pronounce easily.
    /// </summary>
    public class TeacherMemo : Recording
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

		public TeacherMemo()
		{
		}
	}
}