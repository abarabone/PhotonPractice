using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive;
using System.Reactive.Disposables;
using Reactive;
using Reactive.Bindings.Extensions;

namespace EasyPhoton
{
	public class PhotonManager
	{
		
		LoadBalancingClient cl  = new LoadBalancingClient();

		Subject<ClientState>    stateChangeObserver             = new Subject<ClientState>();
		Subject<OperationResponse>  operationResponseObserver   = new Subject<OperationResponse>();

		IDisposable StateChangeDisposer;
		IDisposable OpResponseDisposer;

		CompositeDisposable		multiDisposer = new CompositeDisposable();

		public void Start()
		{
			Observable.Interval( TimeSpan.FromSeconds( 1.0 / 60.0 ) ).Subscribe( _ => cl.Service() )
				.AddTo( multiDisposer );
			
		}

		public void End()
		{
			
			multiDisposer.Dispose();

			Console.WriteLine( "end" );
		}
		
		public async Task JoinAsync()
		{

			var clientStateStream = Observable
				.FromEvent<ClientState>( h => cl.OnStateChangeAction += h, h => cl.OnStateChangeAction -= h );

			clientStateStream.Subscribe( cs => Console.WriteLine( $"@ {cs.ToString()}" ) )
				.AddTo( multiDisposer );


			cl.AppId = "332a6159-ab86-44db-a33e-678409da9e1f";

			cl.ConnectToRegionMaster( "jp" );
			
			await clientStateStream.Where( cs => cs == ClientState.JoinedLobby ).FirstAsync();

			
			var rmopt = new RoomOptions(){ MaxPlayers = 4 };

			cl.OpJoinOrCreateRoom( "room1", rmopt, TypedLobby.Default );
			
			await clientStateStream.Where( cs => cs == ClientState.Joined ).FirstAsync();


			//StartReciving();
			//SendTest();
			
		}

		public void StartReciving()
		{
			var responseStream = Observable
				.FromEvent<OperationResponse>( h => cl.OnOpResponseAction += h, h => cl.OnOpResponseAction -= h );

			responseStream
				.Where( op => op.ReturnCode != 0 )
				.Subscribe( op => Console.WriteLine( $"err : {op}" ) )
				.AddTo( multiDisposer );
			
			responseStream
				.Where( op => op.OperationCode == 1 )
				.Subscribe( op => Console.WriteLine( $"{op.Parameters[0]}" ) )
				.AddTo( multiDisposer );

		}

		public void SendTest()
		{
			var datas = new Dictionary<byte,object>();
			datas.Add( 0, 1 );
			
			cl.loadBalancingPeer.OpCustom( 1, datas, true );
		}

		public void Close()
		{
			cl.Disconnect();

			Console.WriteLine( "discon" );

			Console.ReadKey();
		}

		public void Send()
		{

		}

		public void Recv()
		{

		}

	}

	public class PhotonSerializer
	{
		ExitGames.Client.Photon.SerializeMethod a;
		DeserializeMethod   b;
		void asefe()
		{
			var aa = a( new object() );
			var bb = b( new byte[1] );
		}
		private static short SerializeVector2( StreamBuffer outStream, object customobject )
		{

			return 0;
		}
		private static object DeserializeVector2( StreamBuffer inStream, short length )
		{
			return null;
		}
	}

}
