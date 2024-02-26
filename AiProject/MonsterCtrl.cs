using Godot;
using System;

public enum desiredDirection { Left, Right, Up, Down }

public partial class MonsterCtrl : Sprite2D
{
    public Vector2I currentLocation;
    public Vector2I treasureLocation;
    public Vector2I well1Location;
    public Vector2I well2Location;
    public Vector2I playerLocation;
    private desiredDirection myDesire = desiredDirection.Up;
    public bool MonsterIsDead = false;
    public override void _Ready() { }

    public void DoNextStep()
    {
        //Notify player where Monster location is
        Main.instance.playerScript.monsterLocation = currentLocation;
        //If player is dead, don't move
        if (Main.instance.playerScript.PlayerIsDead)
        {
            Main.instance.AddMessage("M: El intruso murio!");
            Main.points -= 1000;
            return;
        }
        //If the player is at the treasure, don't move
        if (Main.instance.playerScript.PlayerAtTreasure)
        {
            Main.instance.AddMessage("M: El intruso robo mi tesoro!");           
            return;
        }
        //If monster is dead, don't move
        if (MonsterIsDead)
        {
            Main.instance.AddMessage("M: Oh no... eh muerto!");
            return;
        }

        CheckDesire();

        //Move Monster Cordinates & Send Notification
        if (myDesire == desiredDirection.Left) { currentLocation = new Vector2I(currentLocation.X - 1, currentLocation.Y); ClampMove(); Main.instance.AddMessage("M: Muevo a la izquierda"); }
        if (myDesire == desiredDirection.Right) { currentLocation = new Vector2I(currentLocation.X + 1, currentLocation.Y); ClampMove(); Main.instance.AddMessage("M: Muevo a la derecha"); }
        if (myDesire == desiredDirection.Up) { currentLocation = new Vector2I(currentLocation.X, currentLocation.Y - 1); ClampMove(); Main.instance.AddMessage("M: Muevo hacia arriba"); }
        if (myDesire == desiredDirection.Down) { currentLocation = new Vector2I(currentLocation.X, currentLocation.Y + 1); ClampMove(); Main.instance.AddMessage("M: Muevo hacia abajo"); }

        //Move Monster Position
        Position = new Vector2(currentLocation.X * Main.instance.cellSize, currentLocation.Y * Main.instance.cellSize);

        //If the monster is on any of the wells, it died a horrible death
        if (currentLocation == well1Location || currentLocation == well2Location)
        {
            MonsterIsDead = true;
            Main.instance.AddMessage("M: Oh no... eh muerto!");
        }

        //If the monster hits the player after the player moved, mark the player as dead
        if (currentLocation == playerLocation)
        {
            Main.instance.playerScript.PlayerIsDead = true;
            Main.instance.AddMessage("M: El intruso murio!");
        }

    }

    private void ClampMove()
    {
        currentLocation.X = Mathf.Clamp(currentLocation.X, 0, 4);
        currentLocation.Y = Mathf.Clamp(currentLocation.Y, 0, 4);
    }

    private void CheckDesire()
    {
        var myRdm = Main.instance.random.RandiRange(0, 3);

        if ((currentLocation.X + 1 == treasureLocation.X && currentLocation.Y == treasureLocation.Y) || 
		(currentLocation.X - 1 == treasureLocation.X && currentLocation.Y == treasureLocation.Y) ||
		(currentLocation.X == treasureLocation.X && currentLocation.Y - 1 == treasureLocation.Y) ||
		(currentLocation.X == treasureLocation.X && currentLocation.Y + 1 == treasureLocation.Y) ||
		(currentLocation.X + 1 == treasureLocation.X && currentLocation.Y + 1 == treasureLocation.Y) || 
		(currentLocation.X - 1 == treasureLocation.X && currentLocation.Y + 1== treasureLocation.Y) ||
		(currentLocation.X + 1 == treasureLocation.X && currentLocation.Y - 1 == treasureLocation.Y) ||
		(currentLocation.X - 1 == treasureLocation.X && currentLocation.Y - 1 == treasureLocation.Y))
		{
			if (currentLocation.X + 1 == treasureLocation.X) { myDesire = desiredDirection.Left; return; }
			if (currentLocation.X - 1 == treasureLocation.X ) { myDesire = desiredDirection.Right; return; }
			if (currentLocation.Y + 1 == treasureLocation.Y) { myDesire = desiredDirection.Up; return; }
			if (currentLocation.Y - 1 == treasureLocation.Y) { myDesire = desiredDirection.Down; return; }
		}


        //Choose a random direction if not on the edges of the map
        if (currentLocation.Y != 0 && currentLocation.Y != 4 &&
            currentLocation.X != 0 && currentLocation.Y != 4)
        {
            // Evade wells
            if (currentLocation.X + 1 == well1Location.X || currentLocation.X + 1 == well2Location.X) { myDesire = desiredDirection.Left; Main.instance.AddMessage("M: pozo cerca"); return; }
            if (currentLocation.X - 1 == well1Location.X || currentLocation.X - 1 == well2Location.X) { myDesire = desiredDirection.Right; Main.instance.AddMessage("M: pozo cerca"); return; }
            if (currentLocation.Y + 1 == well1Location.Y || currentLocation.Y + 1 == well2Location.Y) { myDesire = desiredDirection.Up; Main.instance.AddMessage("M: pozo cerca"); return; }
            if (currentLocation.Y - 1 == well1Location.Y || currentLocation.Y - 1 == well2Location.Y) { myDesire = desiredDirection.Down; Main.instance.AddMessage("M: pozo cerca"); return; }

            // Avoid treasure
            if (currentLocation.X == treasureLocation.X && currentLocation.Y + 1 == treasureLocation.Y) { myDesire = desiredDirection.Up; Main.instance.AddMessage("M: tesoro cerca"); return; }
            if (currentLocation.X == treasureLocation.X && currentLocation.Y - 1 == treasureLocation.Y) { myDesire = desiredDirection.Down; Main.instance.AddMessage("M: tesoro cerca"); return; }
            if (currentLocation.Y == treasureLocation.Y && currentLocation.X + 1 == treasureLocation.X) { myDesire = desiredDirection.Left; Main.instance.AddMessage("M: tesoro cerca"); return; }
            if (currentLocation.Y == treasureLocation.Y && currentLocation.X - 1 == treasureLocation.X) { myDesire = desiredDirection.Right; Main.instance.AddMessage("M: tesoro cerca"); return; }

            // Find player
            if (currentLocation.X < playerLocation.X) { myDesire = desiredDirection.Right; Main.instance.AddMessage("M: Jugador a mi derecha"); return;}
            else if (currentLocation.X > playerLocation.X) { myDesire = desiredDirection.Left; Main.instance.AddMessage("M: Jugador a mi izquierda"); return;}
            else if (currentLocation.Y < playerLocation.Y) { myDesire = desiredDirection.Up; Main.instance.AddMessage("M: Jugador arriba de mi"); return;}
            else if (currentLocation.Y > playerLocation.Y) { myDesire = desiredDirection.Down; Main.instance.AddMessage("M: Jugador debajo de mi"); return;}
            else
            {
                // If the monster free of risks or temptations, choose a random direction
                if (myRdm == 0) { myDesire = desiredDirection.Left; }
                else if (myRdm == 1) { myDesire = desiredDirection.Right; }
                else if (myRdm == 2) { myDesire = desiredDirection.Up; }
                else if (myRdm == 3) { myDesire = desiredDirection.Down; }
            }
            return;
        }
        
        if (currentLocation.Y == 0 || currentLocation.Y == 4 ||
			currentLocation.X == 0 || currentLocation.Y == 4)
		{
			// Check Edges of the map and avoid dangerous edges
			if (currentLocation.Y == 0) { myDesire = desiredDirection.Down; Main.instance.AddMessage("M: llegue al limite superior del mapa"); return; }
			if (currentLocation.Y == 4) { myDesire = desiredDirection.Up; Main.instance.AddMessage("M: llegue al limite inferior del mapa"); return; }
			if (currentLocation.X == 0) { myDesire = desiredDirection.Right; Main.instance.AddMessage("M: llegue al limite izquierdo del mapa"); return; }
			if (currentLocation.X == 4) { myDesire = desiredDirection.Left; Main.instance.AddMessage("M: llegue al limite derecho del mapa"); return; }

		}
    }





}
