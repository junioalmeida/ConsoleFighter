using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFighter {
    class DeadCharacterException : Exception{

        public Player DeadPlayer { get; private set; }

        public DeadCharacterException() : base () { }
        public DeadCharacterException(string message) : base(message) { }
        public DeadCharacterException(string message, Exception innerException) : base(message, innerException) { }
        public DeadCharacterException(string message, Player player) : base(message){
            DeadPlayer = player;
        }
        public DeadCharacterException(Player player) {
            DeadPlayer = player;
        }

    }
}
