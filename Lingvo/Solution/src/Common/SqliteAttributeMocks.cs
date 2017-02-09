using System;

#if !__MOBILE__
namespace SQLite.Net.Attributes
{
	internal class TableAttribute : Attribute
	{
		public TableAttribute(string tbl) { }
	}
	internal class PrimaryKeyAttribute : Attribute { }
	internal class AutoIncrementAttribute : Attribute { }
}

namespace SQLiteNetExtensions.Attributes
{
	internal class ForeignKeyAttribute : Attribute
	{
		public ForeignKeyAttribute(Type type) { }
	}
	internal class ManyToOneAttribute : Attribute { }
	internal class OneToManyAttribute : Attribute
	{
		public CascadeOperation CascadeOperations { get; set; }
	}
	internal class OneToOneAttribute : Attribute
	{
		public OneToOneAttribute(string key) { }
		public CascadeOperation CascadeOperations { get; set; }
	}
	enum CascadeOperation { All }
}
#endif