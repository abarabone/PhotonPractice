using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;

namespace photonTest
{
	static public class SerializeerPract
	{
		static public void Seri()
		{
			var bs = new byte[30];

			var i = 0;
			ZeroFormatterSerializer.Serialize( ref bs, i, new Seria{ aaa=1, bbb=0.5f } );

			var ser = ZeroFormatterSerializer.Deserialize<Seria>( bs, 0 );

			Console.WriteLine( $"{ser.aaa} {ser.bbb}" );
		}

		[ZeroFormattable]
		public struct Seria
		{
			[Index(0)] public int aaa;
			[Index(1)] public float bbb;
			public Seria( int aaa, float bbb )
			{
				this.aaa = aaa;
				this.bbb = bbb;
			}
		}
	}
}
