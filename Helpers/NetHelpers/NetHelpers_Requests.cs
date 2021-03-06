﻿using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Terraria.ModLoader;


namespace HamstarHelpers.NetHelpers {
	public partial class NetHelpers {
		private readonly static object RequestMutex = new object();



		[System.Obsolete( "use NetHelpers.MakePostRequestAsync(string, byte[], Action<string>, Action<Exception, string>, [Action])", true )]
		public static void MakePostRequestAsync( string url, byte[] bytes, Action<string> on_response, Action<Exception> on_error = null, Action on_completion = null ) {
			Action<Exception, string> on_wrapped_error = ( e, _ ) => {
				if( on_error != null ) {
					on_error( e );
				}
			};

			NetHelpers.MakePostRequestAsync( url, bytes, on_response, on_wrapped_error, on_completion );
		}

		public static void MakePostRequestAsync( string url, byte[] bytes, Action<string> on_response, Action<Exception, string> on_error, Action on_completion=null ) {
			ThreadPool.QueueUserWorkItem( _ => {
				bool success;
				string output = null;

				try {
					//lock( NetHelpers.RequestMutex ) {
					output = NetHelpers.MakePostRequest( url, bytes, out success );
					//}

					if( success ) {
						on_response( output );
					} else {
						output = null;
						throw new Exception( "POST request unsuccessful (url: "+url+")" );
					}
				} catch( Exception e ) {
					if( on_error != null ) {
						on_error( e, output );
					}
				}

				if( on_completion != null ) {
					on_completion();
				}
			} );
		}


		[System.Obsolete( "use NetHelpers.MakePostRequest(string, byte[], out bool)", true )]
		public static string MakePostRequest( string url, byte[] bytes ) {
			bool _;
			return NetHelpers.MakePostRequest( url, bytes, out _ );
		}

		public static string MakePostRequest( string url, byte[] bytes, out bool success ) {
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create( url );
			request.Method = "POST";
			request.ContentType = "application/json";   //"application/vnd.github.v3+json";
			request.ContentLength = bytes.Length;
			request.UserAgent = "tModLoader " + ModLoader.version.ToString();

			using( Stream data_stream = request.GetRequestStream() ) {
				data_stream.Write( bytes, 0, bytes.Length );
				data_stream.Close();
			}

			HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
			int status_code = (int)resp.StatusCode;
			string resp_data;

			success = status_code >= 200 && status_code < 300;

			using( Stream resp_data_stream = resp.GetResponseStream() ) {
				var stream_read = new StreamReader( resp_data_stream, Encoding.UTF8 );
				resp_data = stream_read.ReadToEnd();
				resp_data_stream.Close();
			}

			return resp_data;
		}


		////////////////

		[System.Obsolete( "use NetHelpers.MakeGetRequestAsync(string, Action<string>, Action<Exception, string>, [Action])", true )]
		public static void MakeGetRequestAsync( string url, Action<string> on_response, Action<Exception> on_error = null, Action on_completion = null ) {
			Action<Exception, string> on_wrapped_error = ( e, _ ) => {
				if( on_error != null ) {
					on_error( e );
				}
			};

			NetHelpers.MakeGetRequestAsync( url, on_response, on_wrapped_error, on_completion );
		}

		public static void MakeGetRequestAsync( string url, Action<string> on_response, Action<Exception, string> on_error, Action on_completion = null ) {
			ThreadPool.QueueUserWorkItem( _ => {
				bool success;
				string output = null;

				try {
					//lock( NetHelpers.RequestMutex ) {
					output = NetHelpers.MakeGetRequest( url, out success );
					//}

					if( success ) {
						on_response( output );
					} else {
						output = null;
						throw new Exception( "GET request unsuccessful (url: " + url + ")" );
					}
				} catch( Exception e ) {
					if( on_error != null ) {
						on_error( e, output );
					}
				}

				if( on_completion != null ) {
					on_completion();
				}
			} );
		}


		[System.Obsolete( "use NetHelpers.MakeGetRequest(string, out bool)", true )]
		public static string MakeGetRequest( string url ) {
			bool _;
			return NetHelpers.MakeGetRequest( url, out _ );
		}

		public static string MakeGetRequest( string url, out bool success ) {
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create( url );
			request.Method = "GET";
			request.UserAgent = "tModLoader " + ModLoader.version.ToString();

			HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
			int status_code = (int)resp.StatusCode;
			string resp_data;

			success = status_code >= 200 && status_code < 300;

			using( Stream resp_data_stream = resp.GetResponseStream() ) {
				var stream_read = new StreamReader( resp_data_stream, Encoding.UTF8 );
				resp_data = stream_read.ReadToEnd();
				resp_data_stream.Close();
			}

			return resp_data;
		}
	}
}
