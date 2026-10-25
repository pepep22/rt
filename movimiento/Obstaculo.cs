using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace movimiento
{
    internal class Obstaculo
    {
        public Rectangle Area;
        public bool Activo;

        public Obstaculo(int x, int y, int ancho, int alto)
        {
            Area = new Rectangle(x, y, ancho, alto);
            Activo = true;
        }
    }
}
