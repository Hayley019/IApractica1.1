using Godot;
using System;
using System.Collections.Generic;


public partial class PlayerCtrl : Sprite2D
{
	public Vector2I currentLocation;
	public Vector2I treasureLocation;
	public Vector2I well1Location;
	public Vector2I well2Location;
	public Vector2I monsterLocation;
	public bool PlayerAtTreasure = false;
	public bool PlayerIsDead = false;
	public override void _Ready() { }
	private Vector2I distanceToTreasure = new Vector2I();

	private desiredDirection myDesire = desiredDirection.Up;

	public void DoNextStep()
	{
		//Minus one point for each move
		Main.points -= 1;

		if (PlayerIsDead)
		{
			Main.instance.AddMessage("P: Eh muerto! Game Over.");
			return;
		}

		if (PlayerAtTreasure)
		{
			Main.instance.AddMessage("P: Encontre el tesoro ¡Fantastico!.");			
		}

		CheckDesire();

		//Move Monster Cordinates & Send Notification
		if (myDesire == desiredDirection.Left) { currentLocation = new Vector2I(currentLocation.X - 1, currentLocation.Y); ClampMove(); Main.instance.AddMessage("P: Muevo a la izquierda"); }
		if (myDesire == desiredDirection.Right) { currentLocation = new Vector2I(currentLocation.X + 1, currentLocation.Y); ClampMove(); Main.instance.AddMessage("P: Muevo a la derecha"); }
		if (myDesire == desiredDirection.Up) { currentLocation = new Vector2I(currentLocation.X, currentLocation.Y - 1); ClampMove(); Main.instance.AddMessage("P: Muevo hacia arriba"); }
		if (myDesire == desiredDirection.Down) { currentLocation = new Vector2I(currentLocation.X, currentLocation.Y + 1); ClampMove(); Main.instance.AddMessage("P: Muevo hacia abajo"); }


		//Move the player to the Game Position
		Position = new Vector2(currentLocation.X * Main.instance.cellSize, currentLocation.Y * Main.instance.cellSize);

		//Notify monster the player's new location
		Main.instance.monsterScript.playerLocation = currentLocation;

		//Mark player as dead if the cordinates of the wells or monster is the same as the player
		if (currentLocation == well1Location || currentLocation == well2Location || currentLocation == monsterLocation)
		{
			PlayerIsDead = true;
			Main.instance.AddMessage("P: Eh muerto! Game Over.");
			Main.points -= 1000;
		}

		//Mark player at treasure if cordinates of player and treasure are the same.
		if (currentLocation == treasureLocation)
		{
			PlayerAtTreasure = true;
			Main.instance.AddMessage("P: Encontre el tesoro ¡Fantastico!.");
			Main.points += 1000;
			
		}

	}

	//This function will clamp the player's movement to the map size 
	//The clamp function is used to limit a value to a specific range
	private void ClampMove()
	{
		currentLocation.X = Mathf.Clamp(currentLocation.X, 0, 4);
		currentLocation.Y = Mathf.Clamp(currentLocation.Y, 0, 4);
	}

	private void CheckDesire()
	{
		var myRdm = new Random().Next(0, 3); // Use the Random class instead of RandomNumberGenerator

		// If the treasure is nearby and it's not dangerous (well or monster) go for it
		if ((currentLocation.X + 1 == treasureLocation.X && currentLocation.Y == treasureLocation.Y) || 
		(currentLocation.X - 1 == treasureLocation.X && currentLocation.Y == treasureLocation.Y) ||
		(currentLocation.X == treasureLocation.X && currentLocation.Y - 1 == treasureLocation.Y) ||
		(currentLocation.X == treasureLocation.X && currentLocation.Y + 1 == treasureLocation.Y) ||
		(currentLocation.X + 1 == treasureLocation.X && currentLocation.Y + 1 == treasureLocation.Y) || 
		(currentLocation.X - 1 == treasureLocation.X && currentLocation.Y + 1== treasureLocation.Y) ||
		(currentLocation.X + 1 == treasureLocation.X && currentLocation.Y - 1 == treasureLocation.Y) ||
		(currentLocation.X - 1 == treasureLocation.X && currentLocation.Y - 1 == treasureLocation.Y))
		{
			if (currentLocation.X + 1 == treasureLocation.X && !IsDangerous(currentLocation.X + 1, currentLocation.Y)) { myDesire = desiredDirection.Right; Main.instance.AddMessage("P: Tesoro a mi derecha"); return; }
			if (currentLocation.X - 1 == treasureLocation.X && !IsDangerous(currentLocation.X - 1, currentLocation.Y)) { myDesire = desiredDirection.Left; Main.instance.AddMessage("P: Tesoro a mi izquierda"); return; }
			if (currentLocation.Y + 1 == treasureLocation.Y && !IsDangerous(currentLocation.X, currentLocation.Y + 1)) { myDesire = desiredDirection.Down; Main.instance.AddMessage("P: tesoro abajo"); return; }
			if (currentLocation.Y - 1 == treasureLocation.Y && !IsDangerous(currentLocation.X, currentLocation.Y - 1)) { myDesire = desiredDirection.Up; Main.instance.AddMessage("p: Tesoro arriba"); return; }
		}


		// Choose a random direction if not on the edges of the map and not dangerous
		if (currentLocation.Y != 0 && currentLocation.Y != 4 &&
			currentLocation.X != 0 && currentLocation.Y != 4)
		{
			List<desiredDirection> safeDirections = new List<desiredDirection>();
			if (!IsDangerous(currentLocation.X - 1, currentLocation.Y)) { safeDirections.Add(desiredDirection.Left); }
			if (!IsDangerous(currentLocation.X + 1, currentLocation.Y)) { safeDirections.Add(desiredDirection.Right); }
			if (!IsDangerous(currentLocation.X, currentLocation.Y - 1)) { safeDirections.Add(desiredDirection.Up); }
			if (!IsDangerous(currentLocation.X, currentLocation.Y + 1)) { safeDirections.Add(desiredDirection.Down); }

			if (safeDirections.Count > 0)
			{
				myDesire = safeDirections[new Random().Next(0, safeDirections.Count)]; // Use the Random class here as well
				return;
			}
		}
		if (currentLocation.Y == 0 || currentLocation.Y == 4 ||
			currentLocation.X == 0 || currentLocation.Y == 4)
		{
			// Check Edges of the map and avoid dangerous edges
			if (currentLocation.Y == 0 && !IsDangerous(currentLocation.X, currentLocation.Y + 1)) { myDesire = desiredDirection.Down; Main.instance.AddMessage("P: llegue al limite del mapa"); return; }
			if (currentLocation.Y == 4 && !IsDangerous(currentLocation.X, currentLocation.Y - 1)) { myDesire = desiredDirection.Up; Main.instance.AddMessage("P: llegue al limite del mapa"); return; }
			if (currentLocation.X == 0 && !IsDangerous(currentLocation.X + 1, currentLocation.Y)) { myDesire = desiredDirection.Right; Main.instance.AddMessage("P: llegue al limite del mapa"); return; }
			if (currentLocation.X == 4 && !IsDangerous(currentLocation.X - 1, currentLocation.Y)) { myDesire = desiredDirection.Left; Main.instance.AddMessage("P: llegue al limite del mapa"); return; }

		}

	}

	private bool IsDangerous(int x, int y)
	{
		return (x == monsterLocation.X && y == monsterLocation.Y) ||
			(x == well1Location.X && y == well1Location.Y) ||
			(x == well2Location.X && y == well2Location.Y);
	}

}
