using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Helpers
{
	public static class ReflexionHelper
	{
		public static object SetValueFromKey(object obj, string key, object value)
		{
			string[] keyNodes = key.Split('.');

			MemberInfo currentMemberInfo = DedicatedGameServer.ServerConfig.GetType().GetProperty(keyNodes[1]);
			object parentPropertyValue = DedicatedGameServer.ServerConfig;
			if (keyNodes.Length > 2)
			{
				// For because we skip the first node which contains the class name and
				// we also skip the second one because we used it above
				for (int i = 2; i < keyNodes.Length; i++)
				{
					if (currentMemberInfo is PropertyInfo)
					{
						parentPropertyValue = (currentMemberInfo as PropertyInfo).GetValue(parentPropertyValue);
					}
					else
					{
						parentPropertyValue = (currentMemberInfo as FieldInfo).GetValue(parentPropertyValue);
					}

					currentMemberInfo = parentPropertyValue.GetType().GetProperty(keyNodes[i]);
					if (currentMemberInfo == null)
					{
						currentMemberInfo = parentPropertyValue.GetType().GetField(keyNodes[i]);
					}
				}
			}

			object newValue = null;

			if (currentMemberInfo is PropertyInfo)
			{
				object formattedValue = ChangeType(value, (currentMemberInfo as PropertyInfo).PropertyType);
				(currentMemberInfo as PropertyInfo).SetValue(parentPropertyValue, formattedValue);
				newValue = (currentMemberInfo as PropertyInfo).GetValue(parentPropertyValue);
			}
			else
			{
				object formattedValue = ChangeType(value, (currentMemberInfo as FieldInfo).FieldType);
				(currentMemberInfo as FieldInfo).SetValue(parentPropertyValue, formattedValue);
				newValue = (currentMemberInfo as FieldInfo).GetValue(parentPropertyValue);
			}

			return newValue;
		}


		public static object ChangeType(object value, Type type)
		{
			object formattedValue = null;

			if (type.IsEnum)
			{
				formattedValue = Enum.Parse(type, value.ToString(), true);
			}
			else
			{
				formattedValue = Convert.ChangeType(value, type);
			}

			return formattedValue;
		}
	}
}
