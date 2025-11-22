using System;

namespace ReservasCanchas_Web.Servicios.Exceptions
{
    // Excepción de dominio para cuando el SP detecta solapamiento de reservas
    public class ReservaSolapadaException : Exception
    {
        public ReservaSolapadaException(string message) : base(message) { }
    }
}