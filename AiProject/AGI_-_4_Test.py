import pygame
import random
import math

# Definir colores
WHITE = (255, 255, 255)
BLACK = (0, 0, 0)
RED = (255, 0, 0)
BLUE = (0, 0, 255)
GREEN = (0, 255, 0)

# Tamaño de la pantalla y del tablero
SCREEN_WIDTH = 800
SCREEN_HEIGHT = 500
TILE_SIZE = 100

class AgenteInteligente:
    def __init__(self):
        self.posicion = [0, 0]  # Posición inicial del agente

    def mover(self, direccion): # Método para mover el agente
        if direccion == 'arriba' or direccion == 'ar':
            if self.posicion[0] > 0: # Si la posición del agente es mayor a 0 evitar que se salga del tablero
                self.posicion[0] -= 1
                return True
        elif direccion == 'abajo' or direccion == 'ab':
            if self.posicion[0] < 4:    # Si la posición del agente es menor a 4 evitar que se salga del tablero
                self.posicion[0] += 1
                return True
        elif direccion == 'izquierda' or direccion == 'izq':
            if self.posicion[1] > 0:    # Si la posición del agente es mayor a 0 evitar que se salga del tablero
                self.posicion[1] -= 1
                return True
        elif direccion == 'derecha' or direccion == 'der':
            if self.posicion[1] < 4:    # Si la posición del agente es menor a 4 evitar que se salga del tablero
                self.posicion[1] += 1
                return True
        return False  # Retornar False si no se movió

class Guardian:
    def __init__(self):
        self.posicion = [0, 0]  # Posición inicial del guardian
    
    def posicion_inicial(self, posicion): # Método para la posición inicial del guardian
        self.posicion = posicion
    
    def mover(self, direccion, agente_posicion, tesoro_posicion, pozo1_posicion, pozo2_posicion):
        next_posicion = list(self.posicion)  # Creamos una copia de la posición actual del guardian
        if direccion == 'arriba':
            next_posicion[0] -= 1
        elif direccion == 'abajo':
            next_posicion[0] += 1
        elif direccion == 'izquierda':
            next_posicion[1] -= 1
        elif direccion == 'derecha':
            next_posicion[1] += 1
        
        # Verificar si la próxima posición del guardián coincide con la posición del tesoro o algún pozo
        if next_posicion != tesoro_posicion and next_posicion != pozo1_posicion and next_posicion != pozo2_posicion:
            # Si no coincide con ninguno, mover el guardián a la próxima posición
            self.posicion = next_posicion

class Boton:
    def __init__(self, x, y, texto, funcion):
        self.rect = pygame.Rect(x, y, 150, 40)  # Crea un rectángulo para el botón
        self.color = BLUE  # Color inicial
        self.hovered = False  # Indica si el mouse está sobre el botón
        self.texto = texto  # Texto que se muestra en el botón
        self.funcion = funcion  # Función asociada al botón

    def dibujar(self, pantalla):
        # Cambiar el color si el mouse está sobre el botón
        color = WHITE if self.hovered else self.color
        pygame.draw.rect(pantalla, color, self.rect)
        pygame.draw.rect(pantalla, BLACK, self.rect, 2)

        # Renderizar el texto del botón
        font = pygame.font.Font(None, 24)
        text = font.render(self.texto, True, BLACK)
        text_rect = text.get_rect(center=self.rect.center)
        pantalla.blit(text, text_rect)

    def actualizar(self):
        # Cambiar el color del botón si el mouse está sobre él
        if self.rect.collidepoint(pygame.mouse.get_pos()):
            self.hovered = True
        else:
            self.hovered = False

class Juego:
    def __init__(self):
        pygame.init()
        self.screen = pygame.display.set_mode((SCREEN_WIDTH, SCREEN_HEIGHT))
        pygame.display.set_caption('Juego con PyGame')
        self.clock = pygame.time.Clock()

        self.agente = AgenteInteligente()   # Agente inteligente en el tablero

        self.tesoro = [random.randint(0, 4), random.randint(0, 4)] # Posición del tesoro
        
        # Generar posición del primer pozo, excluyendo la posición inicial del agente
        self.pozo1 = self.generar_posicion_objeto(exclude=[self.agente.posicion])
        
        # Generar posición del segundo pozo, excluyendo la posición del primer pozo y la inicial del agente
        self.pozo2 = self.generar_posicion_objeto(exclude=[self.agente.posicion, self.pozo1])
        
        self.guardian = Guardian()  # Guardian en el tablero
        self.guardian.posicion_inicial(self.generar_posicion_objeto(exclude=[self.agente.posicion, self.tesoro, self.pozo1, self.pozo2]))   # Posición del guardian
        
        self.turno = 0
        self.movimientos_agente = 0
        self.puntuacion = 0  # Variable de puntuación inicializada en cero
        self.energia = 50  # Variable de energía inicializada en 1000

        self.mensajes = []  # Lista para almacenar los mensajes a mostrar

        self.crear_botones()  # Crear botones
    
    def crear_botones(self):
        # Funciones de las acciones de los botones
        def reiniciar_juego():
            # Implementa la lógica para reiniciar el juego aquí
            pass

        def cambiar_velocidad():
            # Implementa la lógica para cambiar la velocidad del juego aquí
            pass

        def pausar_juego():
            # Implementa la lógica para pausar el juego aquí
            pass

        def cambiar_tema():
            # Implementa la lógica para cambiar el tema aquí
            pass

        # Crear los botones
        self.botones = []
        self.botones.append(Boton(SCREEN_WIDTH - 170, SCREEN_HEIGHT - 60, "Reiniciar", reiniciar_juego))
        self.botones.append(Boton(SCREEN_WIDTH - 170, SCREEN_HEIGHT - 120, "Velocidad", cambiar_velocidad))
        self.botones.append(Boton(SCREEN_WIDTH - 170, SCREEN_HEIGHT - 180, "Pausa", pausar_juego))
        self.botones.append(Boton(SCREEN_WIDTH - 170, SCREEN_HEIGHT - 240, "Cambiar Tema", cambiar_tema))

    def calcular_direccion_guardian(self):
        dx = self.agente.posicion[0] - self.guardian.posicion[0]
        dy = self.agente.posicion[1] - self.guardian.posicion[1]

        if abs(dx) > abs(dy):
            if dx > 0:
                return 'abajo'
            else:
                return 'arriba'
        else:
            if dy > 0:
                return 'derecha'
            else:
                return 'izquierda'

    def generar_posicion_objeto(self, exclude=[]):
        while True:
            posicion = [random.randint(0, 4), random.randint(0, 4)]
            if posicion not in exclude and posicion != [0, 0]:  # Excluir la posición inicial del agente
                return posicion
    
    def dibujar_tablero(self):
        self.screen.fill(WHITE)
        for i in range(5):
            for j in range(5):
                pygame.draw.rect(self.screen, BLACK, (j * TILE_SIZE, i * TILE_SIZE, TILE_SIZE, TILE_SIZE), 1)
                if [i, j] == self.agente.posicion:
                    pygame.draw.rect(self.screen, RED, (j * TILE_SIZE, i * TILE_SIZE, TILE_SIZE, TILE_SIZE))
                elif [i, j] == self.tesoro:
                    pygame.draw.rect(self.screen, BLUE, (j * TILE_SIZE, i * TILE_SIZE, TILE_SIZE, TILE_SIZE))
                elif [i, j] == self.pozo1 or [i, j] == self.pozo2:
                    pygame.draw.circle(self.screen, BLACK, (int(j * TILE_SIZE + TILE_SIZE / 2), int(i * TILE_SIZE + TILE_SIZE / 2)), 20)
                elif [i, j] == self.guardian.posicion:
                    pygame.draw.rect(self.screen, GREEN, (j * TILE_SIZE, i * TILE_SIZE, TILE_SIZE, TILE_SIZE))
        pygame.display.flip()

    def verificar_estado(self):
        if self.agente.posicion == self.guardian.posicion:
            self.puntuacion -= 1000  # Si el agente es atrapado por el guardián, se restan 1000 puntos
            return "¡Oh no! ¡Te ha atrapado el guardian!"
        elif self.agente.posicion == self.tesoro:
            self.puntuacion += 1000  # Si el agente encuentra el tesoro, se suman 1000 puntos
            return "¡Felicidades! ¡Has encontrado el tesoro!"
        elif self.agente.posicion == self.pozo1 or self.agente.posicion == self.pozo2:
            self.puntuacion -= 1000  # Si el agente cae en un pozo, se restan 1000 puntos
            return "¡Oh no! ¡Has caído en un pozo!"
        else:
            return None

    def dibujar_puntuacion(self):
        font = pygame.font.Font(None, 36)
        text = font.render("Puntuación: " + str(self.puntuacion), True, BLACK)
        self.screen.blit(text, (SCREEN_WIDTH - text.get_width() - 10, 50))
    
    def dibujar_energia(self):
        font = pygame.font.Font(None, 36)
        text = font.render("Energía: " + str(self.energia), True, BLACK)
        # Cambiar la ubicación del contador de energía al lado derecho
        self.screen.blit(text, (SCREEN_WIDTH - text.get_width() - 10, 10))

    def agregar_mensaje(self, mensaje):
        self.mensajes.append(mensaje)

    def limpiar_mensajes(self):
        self.mensajes = []

    def dibujar_mensajes(self):
        font = pygame.font.Font(None, 24)
        y_offset = 100  # Ajusta el desplazamiento vertical para los mensajes
        for mensaje in self.mensajes:
            text = font.render(mensaje, True, BLACK)
            self.screen.blit(text, (SCREEN_WIDTH - text.get_width() - 10, y_offset))
            y_offset += text.get_height() + 5  # Incrementa el desplazamiento vertical
    
    def manejar_eventos(self):
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                return False
            elif event.type == pygame.KEYDOWN:
                if event.key == pygame.K_UP:
                    if self.agente.mover('arriba'):
                        self.movimientos_agente += 1
                        self.energia -= 1  # Restar 1 a la energía cada vez que el agente se mueva
                elif event.key == pygame.K_DOWN:
                    if self.agente.mover('abajo'):
                        self.movimientos_agente += 1
                        self.energia -= 1  # Restar 1 a la energía cada vez que el agente se mueva
                elif event.key == pygame.K_LEFT:
                    if self.agente.mover('izquierda'):
                        self.movimientos_agente += 1
                        self.energia -= 1  # Restar 1 a la energía cada vez que el agente se mueva
                elif event.key == pygame.K_RIGHT:
                    if self.agente.mover('derecha'):
                        self.movimientos_agente += 1
                        self.energia -= 1  # Restar 1 a la energía cada vez que el agente se mueva

        return True

    def jugar(self):
        running = True
        while running:
            running = self.manejar_eventos()

            resultado = self.verificar_estado()
            if resultado:
                print(resultado)
                self.agregar_mensaje(resultado)
                running = False

            if self.movimientos_agente >= 2:
                self.turno += 1
                # Calcular la dirección hacia la que el guardián debe moverse
                direccion_guardian = self.calcular_direccion_guardian()

                # Mover al guardián en la dirección calculada
                self.guardian.mover(direccion_guardian, self.agente.posicion, self.tesoro, self.pozo1, self.pozo2)

                self.movimientos_agente = 0

            self.dibujar_tablero()
            self.dibujar_puntuacion()  # Dibujar la puntuación en la pantalla
            self.dibujar_energia()  # Dibujar el contador de energía en la pantalla
            self.dibujar_mensajes()  # Agrega esta línea para dibujar los mensajes

            # Dibujar botones
            for boton in self.botones:
                boton.actualizar()
                boton.dibujar(self.screen)

            pygame.display.flip()  # Actualizar la pantalla
            self.clock.tick(60)

        pygame.quit()

juego = Juego()
juego.jugar()
