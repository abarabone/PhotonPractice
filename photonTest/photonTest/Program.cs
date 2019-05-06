using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Client.Photon.LoadBalancing;
using System.Reactive;
using MessagePack;
using SimpleSerialize;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using System.Reactive.Threading.Tasks;
using System.Threading;

namespace photonTest
{
	class Program
	{
		static async Task ent()
		{
			await Task.Delay(3000);
			Console.WriteLine( $"{Thread.CurrentThread.ManagedThreadId} b" );
		}
		static async Task testa()
		{
			Console.WriteLine( $"{Thread.CurrentThread.ManagedThreadId} a");
			await ent();
			Console.WriteLine( $"{Thread.CurrentThread.ManagedThreadId} c" );
		}
		static void Main( string[] args )
		{
			//Console.WriteLine( $"{Thread.CurrentThread.ManagedThreadId} s");
			//testa();
			//Console.WriteLine( $"{Thread.CurrentThread.ManagedThreadId} e" );
			//Console.ReadKey();
			//return;
			try
			{
				abc(); 
			}
			catch( Exception e ){ Console.WriteLine(e.Message);}
			Console.ReadKey();
		}
		void repsub()
		{
			var a = new ReplaySubject<int>();//new AsyncSubject<int>();

			a.OnNext( 1 );
			a.OnNext( 2 );
			a.OnCompleted();
			fff();
			Console.ReadKey();
			
			async void fff()
			{
			//	a.Subscribe( x => Console.WriteLine( $"1subs {x}" ) );
				a.Do(x=>Console.WriteLine(x)).SelectMany(a.Select(x=>x*10)).Subscribe( x => Console.WriteLine( $"2subs {x}" ) );
			//	var i= await a.FirstAsync();
			//	Console.WriteLine( $"end {i}" );
			}
		}
		static async Task abc()
		{
			var ep = new EasyPhoton.PhotonManager();
			ep.Start();
			await ep.JoinAsync();
			ep.StartReciving();
			ep.SendTest();
			await Observable.Interval( TimeSpan.FromSeconds(2) ).FirstAsync();
			ep.Close();
			ep.End();
		}
		void sef()
		{
			var aa = (FloatSerializer)0.155f;

			var se = new SimpleSerializer( 10 );

			se.Write( ( (FloatSerializer)0.155555f ).ToHalf() );

			var bs = se.Bytes;
			Console.WriteLine( $"{bs[0]}+{bs[1]}+{bs[4]}" );

			se.MoveFirst();
			Console.WriteLine( (float)se.Read<HalfSerializer>().ToFloat() );

			Console.ReadKey();
		}

		void aaa()
		{

			var cl = new LoadBalancingClient();

			cl.AppId = "332a6159-ab86-44db-a33e-678409da9e1f";

			cl.ConnectToRegionMaster( "jp" );

			Console.WriteLine( "con" );

			cl.Disconnect();

			Console.WriteLine( "discon" );

			Console.ReadKey();


			var bs = new byte[1024];

			var ms = new System.IO.MemoryStream(bs);

			var i = 0;
			MessagePackBinary.WriteByte( ref bs, i++, (byte)0 );
			MessagePackBinary.WriteByte( ref bs, i++, (byte)1 );
			MessagePackBinary.WriteByte( ref bs, i++, (byte)2 );
			MessagePackBinary.WriteByte( ref bs, i++, (byte)3 );
			MessagePackBinary.WriteByte( ref bs, i++, (byte)4 );
			MessagePackBinary.WriteByte( ref bs, i++, (byte)5 );
			//MessagePackSerializer.Serialize<byte>(ms, 0);
			//MessagePackSerializer.Serialize<byte>(ms, 1);
			//MessagePackSerializer.Serialize<byte>(ms, 2);
			//MessagePackSerializer.Serialize<byte>(ms, 3);
			//MessagePackSerializer.Serialize<byte>(ms, 4);

			i += MessagePackBinary.WriteSingle( ref bs, i, 1.1f );
			i += MessagePackBinary.WriteSingle( ref bs, i, 1.2f );

			MessagePackSerializer.Serialize<float>( ms, 1.0f );
			MessagePackSerializer.Serialize<float>( ms, 1.4f );
			MessagePackSerializer.Serialize<int>( ms, 11 );
			MessagePackSerializer.Serialize<int>( ms, 12 );

			var res = 0;

			Console.WriteLine( MessagePackBinary.ReadSingle( bs, 0, out res ) );
			Console.WriteLine( MessagePackBinary.ReadSingle( bs, 5, out res ) );
			Console.WriteLine( MessagePackBinary.ReadInt32( bs, 10, out res ) );
			Console.WriteLine( MessagePackBinary.ReadInt32( bs, 14, out res ) );
			Console.WriteLine( $"{bs[0]}+{bs[1]}+{bs[4]} {res}" );
			Console.ReadKey();

		}
	}


	public static class Ex
	{
		public static IObservable<T> Where2<T>( this IObservable<T> source, Func<T, bool> creteria )
		{

			//return new AnonymousObservable<T>( onSubscribe );
			return Observable.Create<T>( (Func<IObserver<T>, IDisposable>)onSubscribe );

			IDisposable onSubscribe( IObserver<T> observer )
			{
				IDisposable disposer = null;

				//return disposer = source.Subscribe( new AnonymousObserver<T>( onNext, observer.OnError, observer.OnCompleted ) );
				return disposer = source.Subscribe( Observer.Create<T>( onNext, observer.OnError, observer.OnCompleted ) );

				void onNext( T x )
				{
					try
					{
						if ( !creteria( x ) ) return;
					}
					catch ( Exception e )
					{
						observer.OnError( e );

						disposer.Dispose();

						return;
					}

					observer.OnNext( x );

				}
			}
		}
		public static (int, int) Fplfunc( (int, int) a )
		{
			var b = ( c:a, 1 );
			return b.c;
		}

	}

}
