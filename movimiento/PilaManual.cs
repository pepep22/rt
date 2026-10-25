using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace movimiento
{
    using Microsoft.Xna.Framework;

    public class PilaManual
    {
        private Rectangle[] elementos;
        private int tope;

        public PilaManual(int capacidad)
        {
            elementos = new Rectangle[capacidad];
            tope = -1;
        }

        public int Cantidad
        {
            get { return tope + 1; }
        }

        public bool Push(Rectangle elemento)
        {
            if (tope == elementos.Length - 1)
                return false;

            tope++;
            elementos[tope] = elemento;

            return true;
        }

        public bool Pop(out Rectangle elemento)
        {
            if (tope == -1)
            {
                elemento = default;
                return false;
            }

            elemento = elementos[tope];
            tope--;

            return true;
        }
    }
}
