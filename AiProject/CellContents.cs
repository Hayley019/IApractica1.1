using Godot;

public enum cellTypes { empty, well, player, monster, treasure}

/* Esta clase es la encargada de tomar las casillas del mapa para determinar que tipo de 
casillas son. Toma su tipo, coordenadas y posicion y los almacena para que el resto de scripts
puedan reconocerlos en sus respectivas funciones al momento de ser llamadas.*/
public class CellContents {
	public CellContents(cellTypes type,
						Vector2I coordinates,
						Vector2I position) {
		this.type = type;
		this.coordinates = coordinates;
		this.position = position;
	}

	public cellTypes type;
	public Vector2I coordinates;
	public Vector2I position;
}
