using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFighter {
    class FightController {

        private IWritter Write;
        private Player PlayerLeft;
        private Player PlayerRight;

        public FightController() {
            Write = Writter.GetInstance();

            PlayerLeft = new Player(15, PlayerSide.Left);
            PlayerRight = new Player(102, PlayerSide.Right);
        }

        public void StartFight() {

            ConsoleKeyInfo Key;

            Write.WriteRing();

            try {
                while (true) {
                    Key = Console.ReadKey(true);

                    if (Key.Key == ConsoleKey.D) {

                        if (!(PlayerLeft.Position + 3 == PlayerRight.Position - 2))
                            PlayerLeft.Move(true);
                    } else if (Key.Key == ConsoleKey.A) {

                        PlayerLeft.Move(false);
                    } else if (Key.Key == ConsoleKey.Enter) {

                        int damage = 0;

                        PlayerLeft.Attack();

                        if (PlayerLeft.Position + 3 == PlayerRight.Position - 2) {
                            if (PlayerRight.DefendingState) {
                                damage = 1;
                            } else {
                                damage = 3;
                            }
                        }

                        PlayerRight.ReceiveDamage(damage);
                    } else if(Key.Key == ConsoleKey.Spacebar) {
                        PlayerLeft.Defend();
                    }

                }
            }catch(DeadCharacterException) {
                Console.WriteLine("Player Morto");
                Write.DeadCharacter();
            }
        }
    }
}