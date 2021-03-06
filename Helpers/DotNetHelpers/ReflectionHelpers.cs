﻿using System;
using System.Reflection;


namespace HamstarHelpers.DotNetHelpers {
	public class ReflectionHelpers {
		public static object GetField( Object obj, string field_name, out bool success ) {
			success = false;
			Type objtype = obj.GetType();
			FieldInfo field = objtype.GetField( field_name );

			if( field == null ) { return null; }

			success = true;
			return field.GetValue( obj );
		}

		public static void SetField( Object obj, string field_name, object value, out bool success ) {
			success = false;
			Type objtype = obj.GetType();
			FieldInfo field = objtype.GetField( field_name );

			if( field == null ) { return; }

			success = true;
			field.SetValue( obj, value );
		}


		////////////////

		public static object GetProperty( Object obj, string prop_name, out bool success ) {
			success = false;
			Type objtype = obj.GetType();
			PropertyInfo prop = objtype.GetProperty( prop_name );

			if( prop == null ) { return null; }

			success = true;
			return prop.GetValue( obj );
		}

		public static void SetProperty( Object obj, string prop_name, object value, out bool success ) {
			success = false;
			Type objtype = obj.GetType();
			FieldInfo field = objtype.GetField( prop_name );
			PropertyInfo prop = objtype.GetProperty( prop_name );

			if( prop == null ) { return; }

			success = true;
			prop.SetValue( obj, value );
		}


		////////////////

		public static object RunMethod( Object obj, string method_name, object[] args, out bool success ) {
			success = false;
			Type objtype = obj.GetType();
			MethodInfo method = objtype.GetMethod( method_name );

			if( method != null ) {
				success = true;
				return method.Invoke( obj, args );
			}
			return null;
		}
	}
}
