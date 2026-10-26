using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace movimiento
{
    public struct Disparo
    {
        public Rectangle Rectangulo;
        public int Velocidad;

        public Disparo(int x, int y)
        {
            Rectangulo = new Rectangle(x, y, 6, 16);
            Velocidad = 12;
        }
    }

    public class Game1 : Game
    {
        private Texture2D personaje;
        private Vector2 posicion;
        private float velocidad = 200f;
        private List<Obstaculo> obstaculos = new List<Obstaculo>();

        private PilaManual historialChoques = new PilaManual(20);

        private Texture2D pixel;
        private SpriteFont fuente;
        private int vidas = 3;
        private int puntaje = 0;

        private Texture2D alien;
        private Vector2 posicionAlien;
        private Vector2 velocidadAlien = new Vector2(150f, 120f);
        private bool alienActivo = true;

        private List<Disparo> listaDisparos;
        private KeyboardState teclaActual;
        private KeyboardState teclaAnterior;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 1024;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            listaDisparos = new List<Disparo>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            personaje = Content.Load<Texture2D>("personaje");
            posicion = new Vector2(500, 950);

            alien = Content.Load<Texture2D>("alien");
            posicionAlien = new Vector2(600, 240);

            fuente = Content.Load<SpriteFont>("Arial");
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            obstaculos.Clear();

            //muro
            int anchoObstaculo = 60;
            int altoObstaculo = 20;
            int espacio = 120;

            int distanciaDesdeElFondo = 150;
            int posicionYBarrera = GraphicsDevice.Viewport.Height - distanciaDesdeElFondo;

            for (int posicionX = 0; posicionX < GraphicsDevice.Viewport.Width; posicionX += (anchoObstaculo + espacio))
            {
                obstaculos.Add(new Obstaculo(posicionX, posicionYBarrera, anchoObstaculo, altoObstaculo));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            teclaActual = Keyboard.GetState();
            float tiempo = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // movimiento del alien
            if (alienActivo)
            {
                posicionAlien.X += velocidadAlien.X * tiempo;
                posicionAlien.Y += velocidadAlien.Y * tiempo;

                if (posicionAlien.X > GraphicsDevice.Viewport.Width - alien.Width)
                {
                    posicionAlien.X = GraphicsDevice.Viewport.Width - alien.Width;
                    velocidadAlien.X *= -1;
                }
                else if (posicionAlien.X < 0)
                {
                    posicionAlien.X = 0;
                    velocidadAlien.X *= -1;
                }

                if (posicionAlien.Y > GraphicsDevice.Viewport.Height - alien.Height)
                {
                    posicionAlien.Y = GraphicsDevice.Viewport.Height - alien.Height;
                    velocidadAlien.Y *= -1;
                }
                else if (posicionAlien.Y < 0)
                {
                    posicionAlien.Y = 0;
                    velocidadAlien.Y *= -1;
                }
            }

            // movimiento de la nave
            if (teclaActual.IsKeyDown(Keys.Right) || teclaActual.IsKeyDown(Keys.D))
            {
                posicion.X += velocidad * tiempo;
            }
            if (teclaActual.IsKeyDown(Keys.Left) || teclaActual.IsKeyDown(Keys.A))
            {
                posicion.X -= velocidad * tiempo;
            }
            if (teclaActual.IsKeyDown(Keys.Up) || teclaActual.IsKeyDown(Keys.W))
            {
                posicion.Y -= velocidad * tiempo;
            }
            if (teclaActual.IsKeyDown(Keys.Down) || teclaActual.IsKeyDown(Keys.S))
            {
                posicion.Y += velocidad * tiempo;
            }

            posicion.X = MathHelper.Clamp(posicion.X, 0, GraphicsDevice.Viewport.Width - personaje.Width);
            posicion.Y = MathHelper.Clamp(posicion.Y, 0, GraphicsDevice.Viewport.Height - personaje.Height);

            if (teclaActual.IsKeyDown(Keys.Space) && teclaAnterior.IsKeyUp(Keys.Space))
            {
                int xBalas = (int)posicion.X + (personaje.Width / 2) - 3;
                int yBalas = (int)posicion.Y;
                listaDisparos.Add(new Disparo(xBalas, yBalas));
            }

            for (int i = 0; i < listaDisparos.Count; i++)
            {
                Disparo d = listaDisparos[i];
                d.Rectangulo.Y -= d.Velocidad;
                listaDisparos[i] = d;

                if (d.Rectangulo.Y + d.Rectangulo.Height < 0)
                {
                    listaDisparos.RemoveAt(i);
                    i--;
                }
            }

            //area de personajes
            Rectangle areaPersonaje = new Rectangle(
                (int)posicion.X,
                (int)posicion.Y,
                personaje.Width,
                personaje.Height
            );

            Rectangle areaAlien = new Rectangle(
                (int)posicionAlien.X,
                (int)posicionAlien.Y,
                alien.Width,
                alien.Height
            );


            //colision de balas
            if (alienActivo && areaPersonaje.Intersects(areaAlien))
            {
                vidas--;
                historialChoques.Push(areaAlien);
                alienActivo = false;
            }
            for (int i = 0; i < listaDisparos.Count; i++)
            {
                Disparo d = listaDisparos[i];
                d.Rectangulo.Y -= d.Velocidad;
                listaDisparos[i] = d;

                bool balaEliminada = false;

                if (alienActivo && d.Rectangulo.Intersects(areaAlien))
                {
                    alienActivo = false;
                    puntaje += 10;
                    listaDisparos.RemoveAt(i);
                    i--;
                    continue;
                }

                for (int j = 0; j < obstaculos.Count; j++)
                {
                    if (obstaculos[j].Activo && d.Rectangulo.Intersects(obstaculos[j].Area))
                    {
                        listaDisparos.RemoveAt(i);
                        i--;
                        balaEliminada = true;
                        break;
                    }
                }

                if (balaEliminada) continue;

                if (d.Rectangulo.Y + d.Rectangulo.Height < 0)
                {
                    listaDisparos.RemoveAt(i);
                    i--;
                }
            }

            //colosion del alien
            if (alienActivo)
            {
                for (int i = 0; i < obstaculos.Count; i++)
                {
                    Obstaculo obstaculo = obstaculos[i];

                    if (!obstaculo.Activo)
                        continue;

                    if (areaAlien.Intersects(obstaculo.Area))
                    {
                        Rectangle interseccion = Rectangle.Intersect(areaAlien, obstaculo.Area);

                        if (interseccion.Width > interseccion.Height)
                        {
                            velocidadAlien.Y *= -1;

                            if (posicionAlien.Y < obstaculo.Area.Y)
                                posicionAlien.Y = obstaculo.Area.Y - alien.Height;
                            else
                                posicionAlien.Y = obstaculo.Area.Bottom;
                        }
                        else
                        {
                            velocidadAlien.X *= -1;

                            if (posicionAlien.X < obstaculo.Area.X)
                                posicionAlien.X = obstaculo.Area.X - alien.Width;
                            else
                                posicionAlien.X = obstaculo.Area.Right;
                        }

                        historialChoques.Push(obstaculo.Area);
                        obstaculo.Activo = false;
                        break;
                    }
                }
            }

            // Guardar el estado del teclado para el siguiente ciclo
            teclaAnterior = teclaActual;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(10, 20, 40));

            _spriteBatch.Begin();

            //dibujar personajes
            _spriteBatch.Draw(
                personaje,
                posicion,
                Color.White
            );

            if (alienActivo)
            {
                _spriteBatch.Draw(
                    alien,
                    posicionAlien,
                    Color.White
                );
            }

            // Dibujar los disparos
            foreach (Disparo disparo in listaDisparos)
            {
                _spriteBatch.Draw(pixel, disparo.Rectangulo, Color.Cyan);
            }

            // Dibujar vidas
            _spriteBatch.DrawString(
                fuente,
                "Vidas: " + vidas,
                new Vector2(10, 10),
                Color.White
            );

            // Dibujar puntaje
            _spriteBatch.DrawString(
                fuente,
                "Puntaje: " + puntaje,
                new Vector2(10, 70),
                Color.Yellow
            );

            // Dibujar obstáculos activos
            foreach (Obstaculo obstaculo in obstaculos)
            {
                if (obstaculo.Activo)
                {
                    _spriteBatch.Draw(
                        pixel,
                        obstaculo.Area,
                        Color.Red
                    );
                }
            }

            // Mostrar registros de la pila
            _spriteBatch.DrawString(
            fuente,
            "Choques registrados: " + historialChoques.Cantidad,
            new Vector2(10, 40),
            Color.Yellow
            );
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
