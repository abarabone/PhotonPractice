﻿using System;
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
using ZeroFormatter;

namespace EasyPhoton
{
	/// <summary>
	/// https://doc-api.photonengine.com/en/dotnet/current/
	/// </summary>
	public class PhotonManager
	{
		
		LoadBalancingClient cl  = new LoadBalancingClient();

		Subject<ClientState>		stateChangeObserver			= new Subject<ClientState>();
		Subject<OperationResponse>  operationResponseObserver   = new Subject<OperationResponse>();

		//IDisposable StateChangeDisposer;
		//IDisposable OpResponseDisposer;

		CompositeDisposable		compositeDisposer = new CompositeDisposable();

		public void Start()
		{
			Observable
				.Interval( TimeSpan.FromSeconds( 1.0 / 60.0 ) )
				.Subscribe( _ => this.cl.Service() )
				.AddTo( this.compositeDisposer );
			
		}

		public void End()
		{
			
			Console.WriteLine( "end" );
			this.compositeDisposer.Dispose();
			
			//Console.ReadKey();
			//await obse
		}
		
		public async Task JoinAsync()
		{

			var clientStateStream = Observable
				.FromEvent<ClientState>( h => this.cl.OnStateChangeAction += h, h => this.cl.OnStateChangeAction -= h );

			clientStateStream
				.Subscribe( cs => Console.WriteLine( $"state @ {cs.ToString()}" ) )
				.AddTo( this.compositeDisposer );


			this.cl.AppId = "332a6159-ab86-44db-a33e-678409da9e1f";

			this.cl.ConnectToRegionMaster( "jp" );
			Console.WriteLine( $"ConnectToRegionMaster" );
			
			await clientStateStream
				.Where( cs => cs == ClientState.JoinedLobby )
				.FirstAsync();

			
			var rmopt = new RoomOptions(){ MaxPlayers = 4 };

			this.cl.OpJoinOrCreateRoom( "room1", rmopt, TypedLobby.Default );
			Console.WriteLine( $"OpJoinOrCreateRoom" );
			
			await clientStateStream
				.Where( cs => cs == ClientState.Joined )
				.FirstAsync();


			StartReciving();
			SendTest();
			
			await Observable.Interval( TimeSpan.FromSeconds(5) ).FirstAsync();
			
			this.cl.OpLeaveRoom();
			await clientStateStream
				.Where( cs => cs == ClientState.JoinedLobby )
				.FirstAsync();
			this.cl.OpLeaveLobby();
			
			await Observable.Interval( TimeSpan.FromSeconds(3) ).FirstAsync();
		}

		public void StartReciving()
		{
			var responseStream = Observable
				.FromEvent<OperationResponse>( h => this.cl.OnOpResponseAction += h, h => this.cl.OnOpResponseAction -= h );

			responseStream
				.Where( op => op.ReturnCode != 0 )
				.Subscribe( op => Console.WriteLine( $"err : {op}" ) )
				.AddTo( this.compositeDisposer );

			responseStream
				//.Where( op => op.OperationCode == OperationCode.RaiseEvent )
				.Subscribe( op => Console.WriteLine( $"response {op.OperationCode} {op.Parameters.Count}" ) )
				.AddTo( this.compositeDisposer );


			var eventStream = Observable
				.FromEvent<EventData>( h => this.cl.OnEventAction += h, h => this.cl.OnEventAction -= h );

			eventStream
				.Subscribe( ev => Console.WriteLine( $"event {ev.Code} {ev.Parameters.Count}" ) )
				.AddTo( this.compositeDisposer );
		}

		public void SendTest()
		{
			var bs = new byte[1024];
			ZeroFormatterSerializer.Serialize( ref bs, 0, 11 );
			ZeroFormatterSerializer.Serialize( ref bs, 4, 11 );

			var datas = new Dictionary<byte,object>();
			datas[ ParameterCode.Data ] = bs;
			datas[ParameterCode.ReceiverGroup] = ReceiverGroup.All;

			//this.cl.OpRaiseEvent( 0, 112, true, new RaiseEventOptions{Receivers = ReceiverGroup.All } );
			//this.cl.loadBalancingPeer.OpRaiseEvent( 0, 113, true, RaiseEventOptions.Default );
			datas[ ParameterCode.Code ] = (byte)0;
			this.cl.loadBalancingPeer.OpCustom( customOpCode: OperationCode.RaiseEvent, customOpParameters: datas, sendReliable: false );
			datas[ ParameterCode.Code ] = (byte)1;
			this.cl.loadBalancingPeer.OpCustom( customOpCode: OperationCode.RaiseEvent, customOpParameters: datas, sendReliable: false );
			datas[ ParameterCode.Code ] = (byte)2;
			this.cl.loadBalancingPeer.OpCustom( customOpCode: OperationCode.RaiseEvent, customOpParameters: datas, sendReliable: false );
		}

		public void Close()
		{
			Console.WriteLine( "discon" );
			this.cl.Disconnect();


			//Console.ReadKey();
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
