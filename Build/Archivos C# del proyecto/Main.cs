using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public partial class Main : Node2D
{
	//Declaracion de nuestras variables
	private int playerMoves = 0;
	private int speed = 0;
	public static int points = 1100;
	public static Main instance;

	//Busca el archivo PlayerCtrl.cs y MonsterCtrl.cs para que se puedan usar en este archivo
	[Export] public PlayerCtrl playerScript;
	[Export] public MonsterCtrl monsterScript;
	private bool pausa = false;

	//Busca los componentes en la interfaz de godot para manipularlos.
	private Sprite2D well1, well2, player, monster, treasure;
	private TextEdit msgBox;
	private Label pointsLabel;
	//Declaracion de las coordenadas de las casillas del mapa
	private Vector2I r00 = new Vector2I(0, 0);
	private Vector2I r01 = new Vector2I(1, 0);
	private Vector2I r02 = new Vector2I(2, 0);
	private Vector2I r03 = new Vector2I(3, 0);
	private Vector2I r04 = new Vector2I(4, 0);

	private Vector2I r10 = new Vector2I(0, 1);
	private Vector2I r11 = new Vector2I(1, 1);
	private Vector2I r12 = new Vector2I(2, 1);
	private Vector2I r13 = new Vector2I(3, 1);
	private Vector2I r14 = new Vector2I(4, 1);

	private Vector2I r20 = new Vector2I(0, 2);
	private Vector2I r21 = new Vector2I(1, 2);
	private Vector2I r22 = new Vector2I(2, 2);
	private Vector2I r23 = new Vector2I(3, 2);
	private Vector2I r24 = new Vector2I(4, 2);

	private Vector2I r30 = new Vector2I(0, 3);
	private Vector2I r31 = new Vector2I(1, 3);
	private Vector2I r32 = new Vector2I(2, 3);
	private Vector2I r33 = new Vector2I(3, 3);
	private Vector2I r34 = new Vector2I(4, 3);

	private Vector2I r40 = new Vector2I(0, 4);
	private Vector2I r41 = new Vector2I(1, 4);
	private Vector2I r42 = new Vector2I(2, 4);
	private Vector2I r43 = new Vector2I(3, 4);
	private Vector2I r44 = new Vector2I(4, 4);

	//Crea un diccionario con todas las direcciones del mapa.
	private Dictionary<int, CellContents> allRows = new Dictionary<int, CellContents>();

	//Crea un generador de numeros aleatorios
	public RandomNumberGenerator random = new RandomNumberGenerator();
	//Crea una variable para el tamaño de las casillas
	public int cellSize = 32;

	//Funcion que se ejecuta al iniciar el juego
	public override void _Ready()
	{
		//Inicializa la variable instance
		instance = this;
		// Llena el diccionario con las casillas del mapa
		allRows.Add(0, new CellContents(cellTypes.empty, r00, new Vector2I(r00.X * cellSize, r00.Y * cellSize)));
		allRows.Add(1, new CellContents(cellTypes.empty, r01, new Vector2I(r01.X * cellSize, r01.Y * cellSize)));
		allRows.Add(2, new CellContents(cellTypes.empty, r02, new Vector2I(r02.X * cellSize, r02.Y * cellSize)));
		allRows.Add(3, new CellContents(cellTypes.empty, r03, new Vector2I(r03.X * cellSize, r03.Y * cellSize)));
		allRows.Add(4, new CellContents(cellTypes.empty, r04, new Vector2I(r04.X * cellSize, r04.Y * cellSize)));

		allRows.Add(5, new CellContents(cellTypes.empty, r10, new Vector2I(r10.X * cellSize, r10.Y * cellSize)));
		allRows.Add(6, new CellContents(cellTypes.empty, r11, new Vector2I(r11.X * cellSize, r11.Y * cellSize)));
		allRows.Add(7, new CellContents(cellTypes.empty, r12, new Vector2I(r12.X * cellSize, r12.Y * cellSize)));
		allRows.Add(8, new CellContents(cellTypes.empty, r13, new Vector2I(r13.X * cellSize, r13.Y * cellSize)));
		allRows.Add(9, new CellContents(cellTypes.empty, r14, new Vector2I(r14.X * cellSize, r14.Y * cellSize)));

		allRows.Add(10, new CellContents(cellTypes.empty, r20, new Vector2I(r20.X * cellSize, r20.Y * cellSize)));
		allRows.Add(11, new CellContents(cellTypes.empty, r21, new Vector2I(r21.X * cellSize, r21.Y * cellSize)));
		allRows.Add(12, new CellContents(cellTypes.empty, r22, new Vector2I(r22.X * cellSize, r22.Y * cellSize)));
		allRows.Add(13, new CellContents(cellTypes.empty, r23, new Vector2I(r23.X * cellSize, r23.Y * cellSize)));
		allRows.Add(14, new CellContents(cellTypes.empty, r24, new Vector2I(r24.X * cellSize, r24.Y * cellSize)));

		allRows.Add(15, new CellContents(cellTypes.empty, r30, new Vector2I(r30.X * cellSize, r30.Y * cellSize)));
		allRows.Add(16, new CellContents(cellTypes.empty, r31, new Vector2I(r31.X * cellSize, r31.Y * cellSize)));
		allRows.Add(17, new CellContents(cellTypes.empty, r32, new Vector2I(r32.X * cellSize, r32.Y * cellSize)));
		allRows.Add(18, new CellContents(cellTypes.empty, r33, new Vector2I(r33.X * cellSize, r33.Y * cellSize)));
		allRows.Add(19, new CellContents(cellTypes.empty, r34, new Vector2I(r34.X * cellSize, r34.Y * cellSize)));

		allRows.Add(20, new CellContents(cellTypes.empty, r40, new Vector2I(r40.X * cellSize, r40.Y * cellSize)));
		allRows.Add(21, new CellContents(cellTypes.empty, r41, new Vector2I(r41.X * cellSize, r41.Y * cellSize)));
		allRows.Add(22, new CellContents(cellTypes.empty, r42, new Vector2I(r42.X * cellSize, r42.Y * cellSize)));
		allRows.Add(23, new CellContents(cellTypes.empty, r43, new Vector2I(r43.X * cellSize, r43.Y * cellSize)));
		allRows.Add(24, new CellContents(cellTypes.empty, r44, new Vector2I(r44.X * cellSize, r44.Y * cellSize)));

		//Busca los componentes en la interfaz de godot para manipularlos.
		well1 = GetNode<Sprite2D>("Well1");
		well2 = GetNode<Sprite2D>("Well2");
		player = GetNode<Sprite2D>("Player");
		monster = GetNode<Sprite2D>("Monster");
		treasure = GetNode<Sprite2D>("Treasure");
		msgBox = GetNode<TextEdit>("CanvasLayer/Control/TextEdit");
		pointsLabel = GetNode<Label>("CanvasLayer/Control/Label2");

		//Inicializa el juego
		ResetGame();
	}

	//Funcion que reinicia el juego
	private void ResetGame()
	{
		playerMoves = 0;
		points = 1100;
		//Clear the Message Box, limpia el cuadro de mensajes.
		ResetMessageBox();

		//Place the Player at 4,0. Coloca al jugador en su posicion inicial.
		player.Position = allRows[20].position;

		//Randomly Choose a Coordinate. Elige aleatoriamente una coordenada.
		//No comparison needed to find position since this is the first object to be placed. 
		//No se necesita comparacion para encontrar la posicion ya que este es el primer objeto que se coloca.
		var well1Rdm = random.RandiRange(0, 24);

		while (well1Rdm == 15 || well1Rdm == 16 || well1Rdm == 20 || well1Rdm == 21) { well1Rdm = random.RandiRange(0, 24); }
		//Compare the positions chosen for these objects so that they do not sit on each other. 
		//Compara las posiciones elegidas para estos objetos para que no se sienten uno sobre el otro.
		var well2Rdm = random.RandiRange(1, 24);
		var monsterRdm = random.RandiRange(1, 24);
		var treasureRdm = random.RandiRange(1, 24);

		//Choose a different cell than what well1 was.
		//Elige una celda diferente a la que estaba well1.
		while (well2Rdm == well1Rdm || well2Rdm == 15 || well2Rdm == 16 || well2Rdm == 20 || well2Rdm == 21) { well2Rdm = random.RandiRange(0, 24); }

		//Choose a different cell than well1 and well2
		//Elige una celda diferente a la que estaba well1 y well2.
		while (monsterRdm == well1Rdm || monsterRdm == well2Rdm || monsterRdm == 15 || monsterRdm == 16 || monsterRdm == 20 || monsterRdm == 21) { monsterRdm = random.RandiRange(1, 24); }

		//Choose a different cell than well1, well2, and monster
		//Elige una celda diferente a la que estaba well1, well2 y monster.
		while (treasureRdm == well1Rdm || treasureRdm == well2Rdm || treasureRdm == monsterRdm || treasureRdm == 15 || treasureRdm == 16 || treasureRdm == 20 || well2Rdm == 21) { treasureRdm = random.RandiRange(1, 24); }

		//Place Well1 into Position and Mark the Cell
		//Coloca Well1 en la posicion y marca la celda.        
		well1.Position = allRows[well1Rdm].position;
		allRows[well1Rdm].type = cellTypes.well;
		var well1Cords = allRows[well1Rdm].coordinates;

		//Place Well2 into Position and Mark the Cell
		//Coloca Well2 en la posicion y marca la celda.
		well2.Position = allRows[well2Rdm].position;
		allRows[well2Rdm].type = cellTypes.well;
		var well2Cords = allRows[well2Rdm].coordinates;

		//Place Monster into Position and Mark the Cell
		//Coloca al monstruo en la posicion y marca la celda.
		monster.Position = allRows[monsterRdm].position;
		allRows[monsterRdm].type = cellTypes.monster;
		var monsterCords = allRows[monsterRdm].coordinates;

		//Place Treasure into Position and Mark the cell
		//Coloca el tesoro en la posicion y marca la celda.
		treasure.Position = allRows[treasureRdm].position;
		allRows[treasureRdm].type = cellTypes.treasure;
		var treasureCords = allRows[treasureRdm].coordinates;

		//Player Script init
		//Inicializa el script del jugador
		playerScript.PlayerAtTreasure = false;
		playerScript.PlayerIsDead = false;
		playerScript.currentLocation = allRows[20].coordinates;
		playerScript.treasureLocation = allRows[treasureRdm].coordinates;

		playerScript.well1Location = allRows[well1Rdm].coordinates;
		playerScript.well2Location = allRows[well2Rdm].coordinates;
		playerScript.monsterLocation = allRows[monsterRdm].coordinates;

		//Monster Script init
		//Inicializa el script del monstruo
		monsterScript.MonsterIsDead = false;
		monsterScript.currentLocation = allRows[monsterRdm].coordinates;

		monsterScript.well1Location = allRows[well1Rdm].coordinates;
		monsterScript.well2Location = allRows[well2Rdm].coordinates;
		monsterScript.playerLocation = allRows[20].coordinates;


		//Show Initilization Messages
		//Muestra los mensajes de inicializacion
		AddMessage("Posision pozo1: " + well1Cords);
		AddMessage("Posision pozo1: " + well2Cords);
		AddMessage("Posision Gumpy: " + monsterCords);
		AddMessage("Posision Tesoro: " + treasureCords);
		AddMessage("Inicializacion completa.");
	}

	//Funcion que limpia el cuadro de mensajes
	private void ResetMessageBox()
	{
		messageList.Clear();
		msgBox.Text = string.Empty;
		pointsLabel.Text = string.Empty;
	}


	//Crea una lista de mensajes y un maximo de mensajes
	private List<string> messageList = new List<string>();
	private int maxMessages = 11;
	//Funcion que agrega mensajes al cuadro de mensajes
	public void AddMessage(string myStr)
	{

		//Add message sent as argument to the list
		//Agrega el mensaje enviado como argumento a la lista
		messageList.Add(myStr + "\n");

		//If too many messages, delete the oldest message;
		//Si hay demasiados mensajes, elimina el mensaje mas antiguo;
		if (messageList.Count >= maxMessages) { messageList.RemoveAt(0); }

		//Clear message box then put array into message box
		//Limpia el cuadro de mensajes y luego pone el array en el cuadro de mensajes
		msgBox.Text = string.Empty;
		foreach (var item in messageList)
		{
			msgBox.Text += item;
		}
	}
	//Funcion que se ejecuta al presionar el boton de pausa. Activa la pausa para detener el juego.
	public async void _on_next_move_button_up()
	{
		pausa = true;        
	}
	//Funcion que se ejecuta al presionar el boton de velocidad1. Activa la velocidad 1 para el juego.
	public async void _on_velocidad_1_button_up()
	{
		DelayedSequence(2000);
	}
	//Funcion que se ejecuta al presionar el boton de velocidad2. Activa la velocidad 2 para el juego.
	public async void _on_velocidad_2_button_up()
	{
		DelayedSequence(1500);
	}
	//Funcion que se ejecuta al presionar el boton de velocidad3. Activa la velocidad 3 para el juego.
	public async void _on_velocidad_3_button_up()
	{   
		DelayedSequence(1000);
	}
	//Es la funcion a la que llaman los botones para accionar el juego de forma automatica. 
	private async Task DelayedSequence(int velocidad)
	{
		pausa = false;
		while (!playerScript.PlayerAtTreasure && !playerScript.PlayerIsDead && pausa == false)
		{			
			// Delay for 1 second
			//Espera 1 segundo
			await Task.Delay(velocidad);
			//Do Player AI
			//Hace el AI del jugador
			playerScript.DoNextStep();
			playerMoves++;
			//Let player move 2 times, then let the monster move 1 time
			//Deja que el jugador se mueva 2 veces, luego deja que el monstruo se mueva 1 vez
			if (playerMoves > 1)
			{
				//Do Monster AI
				//Hace el AI del monstruo
				monsterScript.DoNextStep();
				//Reset player moves.
				//Reinicia los movimientos del jugador.
				playerMoves = 0;
			}
			//Update Points each turn
			pointsLabel.Text = "Puntos: " + points;
		}
		pausa = true;
	}

}

