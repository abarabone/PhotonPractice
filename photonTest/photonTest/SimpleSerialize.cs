using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace SimpleSerialize
{

	public class SimpleSerializer
	{

		public byte[]	Bytes { get; private set; }

		private uint	offset;


		public SimpleSerializer( uint byteSize )
		{
			this.Bytes	= new byte[byteSize];
			this.offset	= 0;
		}

		public void Write<T>( T serializedValue ) where T : struct, ISimpleSerializer
		{
			this.offset += serializedValue.WriteTo( this.Bytes, this.offset );
		}

		public T Read<T>() where T : struct, ISimpleSerializer
		{
			var se      =  new T();
			this.offset += se.ReadFrom( this.Bytes, this.offset );

			return se;
		}

		public void MoveFirst()
		{
			this.offset = 0;
		}
	}


	public interface ISimpleSerializer//ISinSerializable
	{
		uint WriteTo( byte[] bytes, uint offset );
		uint ReadFrom( byte[] bytes, uint offset );
	}

	[StructLayout( LayoutKind.Explicit )]
	public struct FloatSerializer : ISimpleSerializer 
	{

		[FieldOffset(0)]
		private float	floatValue;

		[FieldOffset(0)]
		private uint	uintValue;


		[FieldOffset(0)]	private byte	b0;
		[FieldOffset(1)]	private byte	b1;
		[FieldOffset(2)]	private byte	b2;
		[FieldOffset(3)]	private byte	b3;


		uint ISimpleSerializer.WriteTo( byte[] bytes, uint offset )
		{
			bytes[offset + 0] = b0;
			bytes[offset + 1] = b1;
			bytes[offset + 2] = b2;
			bytes[offset + 3] = b3;

			return 4;
		}

		uint ISimpleSerializer.ReadFrom( byte[] bytes, uint offset )
		{
			b0 = bytes[offset + 0];
			b1 = bytes[offset + 1];
			b2 = bytes[offset + 2];
			b3 = bytes[offset + 3];

			return 4;
		}

		public HalfSerializer ToHalf()
		{
			var n        = uintValue;
			var sign     = ( n >> 16 ) & 0x8000;
			var exponent = ( ( (n >> 23) - 127 + 15 ) & 0x1f ) << 10;
			var fraction = ( n >> (23-10) ) & 0x3ff;
			
			return (HalfSerializer)( sign | exponent | fraction );
		}


		public static explicit operator FloatSerializer( float value )
		{
			var se			= new FloatSerializer();
			se.floatValue	= value;

			return se;
		}
		public static explicit operator float( FloatSerializer value )
		{
			return value.floatValue;
		}

		public static explicit operator FloatSerializer( uint value )
		{
			var se          = new FloatSerializer();
			se.uintValue	= value;

			return se;
		}
		public static explicit operator uint( FloatSerializer value )
		{
			return value.uintValue;
		}

	}

	[StructLayout( LayoutKind.Explicit )]
	public struct HalfSerializer : ISimpleSerializer
	{

		[FieldOffset(0)]
		private ushort  halfValue;

		[FieldOffset(0)]
		private ushort  ushortValue;


		[FieldOffset(0)]    private byte    b0;
		[FieldOffset(1)]    private byte    b1;


		public FloatSerializer ToFloat()
		{
			var n        = (uint)ushortValue;
			var sign	 = ( n & 0x8000 ) << 16;
			var exponent = ( ( ((n >> 10) & 0x1f ) - 15 + 127) & 0xff ) << 23;
			var fraction = ( n & 0x3ff ) << (23 - 10);
			
			return (FloatSerializer)( sign | exponent | fraction );
		}
		

		uint ISimpleSerializer.WriteTo( byte[] bytes, uint offset )
		{
			bytes[offset + 0] = b0;
			bytes[offset + 1] = b1;

			return 2;
		}
		uint ISimpleSerializer.ReadFrom( byte[] bytes, uint offset )
		{
			b0 = bytes[offset + 0];
			b1 = bytes[offset + 1];

			return 2;
		}

		public static explicit operator HalfSerializer( ushort value )
		{
			var se          = new HalfSerializer();
			se.halfValue	= value;

			return se;
		}
		public static explicit operator ushort( HalfSerializer value )
		{
			return value.halfValue;
		}
	}

	[StructLayout( LayoutKind.Explicit )]
	public struct UintSerializer : ISimpleSerializer
	{
		
		[FieldOffset(0)]
		private uint    uintValue;


		[FieldOffset(0)]    private byte    b0;
		[FieldOffset(1)]    private byte    b1;
		[FieldOffset(2)]    private byte    b2;
		[FieldOffset(3)]    private byte    b3;


		uint ISimpleSerializer.WriteTo( byte[] bytes, uint offset )
		{
			bytes[offset + 0] = b0;
			bytes[offset + 1] = b1;
			bytes[offset + 2] = b2;
			bytes[offset + 3] = b3;

			return 4;
		}

		uint ISimpleSerializer.ReadFrom( byte[] bytes, uint offset )
		{
			b0 = bytes[offset + 0];
			b1 = bytes[offset + 1];
			b2 = bytes[offset + 2];
			b3 = bytes[offset + 3];

			return 4;
		}

		
		public static explicit operator UintSerializer( uint value )
		{
			var se			= new UintSerializer();
			se.uintValue	= value;

			return se;
		}
		public static explicit operator uint( UintSerializer value )
		{
			return value.uintValue;
		}

	}

	[StructLayout( LayoutKind.Explicit )]
	public struct UshortSerializer : ISimpleSerializer
	{

		[FieldOffset(0)]
		private ushort	ushortValue;
		

		[FieldOffset(0)]	private byte	b0;
		[FieldOffset(1)]	private byte	b1;


		uint ISimpleSerializer.WriteTo( byte[] bytes, uint offset )
		{
			bytes[offset + 0] = b0;
			bytes[offset + 1] = b1;

			return 2;
		}
		uint ISimpleSerializer.ReadFrom( byte[] bytes, uint offset )
		{
			b0 = bytes[offset + 0];
			b1 = bytes[offset + 1];

			return 2;
		}

		public static explicit operator UshortSerializer( ushort value )
		{
			var se			= new UshortSerializer();
			se.ushortValue	= value;

			return se;
		}
		public static explicit operator ushort( UshortSerializer value )
		{
			return value.ushortValue;
		}
	}

}
