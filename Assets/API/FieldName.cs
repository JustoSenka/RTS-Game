using System;
using System.Reflection;

[System.AttributeUsage(System.AttributeTargets.Field)]
public class FieldName : System.Attribute
{
	public string name;

	public FieldName(string name)
	{
		this.name = name;
	}
}
