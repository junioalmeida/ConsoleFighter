using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleFighter {
    class FightController {

        private readonly object SyncRing;
        private readonly IWritter Write;

        private readonly Player PlayerLeft;
        private readonly Player PlayerRight;

        private Thread ResizeArena;
        private Thread IA;
        private Thread Gamer;

        private int NewWidth;

        private int Together;

        public FightController() {

            SyncRing = new object();
            Write = Writter.GetInstance();

            PlayerLeft = new Player(15, PlayerSide.Left, 40);
            PlayerRight = new Player(102, PlayerSide.Right, 40);

            NewWidth = 119;
        }

        private void DecreaseArena() {

            bool WasDefending = false;

            while (NewWidth > 90 && Player.PlayerDied == null) {

                Thread.Sleep(1000);

                lock (SyncRing) {
                    if (Write.IsAtEdge(PlayerLeft.Position)) {

                        if (PlayerLeft.DefendingState)
                            WasDefending = true;

                        PlayerLeft.Move(true);

                        if (WasDefending)
                            PlayerLeft.Defend();

                        WasDefending = false;

                        if (Together == 1)
                            PlayerRight.Move(true);
                    }
                    if (Write.IsAtEdge(PlayerRight.Position)) {

                        if (PlayerRight.DefendingState)
                            WasDefending = true;

                        PlayerRight.Move(false);

                        if (WasDefending)
                            PlayerRight.Defend();

                        WasDefending = false;

                        if (Together == 1)
                            PlayerLeft.Move(false);
                    }

                    if (Player.PlayerDied == null)
                        Write.ResizeRing(NewWidth--);
                }
            }
        }

        private void StartIA() {

            Task PlayerAttack;
            Thread.Sleep(1000);

            Random X = new Random();

            int Attacked = -1;

            while (Player.PlayerDied == null) {

                Thread.Sleep(50);

                PlayerRight.Defend();

                lock (SyncRing) {
                    if (PlayerLeft.Position + 3 != PlayerRight.Position - 2) {
                        Interlocked.Exchange(ref Together, PlayerRight.Move(false));
                    }
                }


                if (Together == 1) {

                    lock (SyncRing) {
                        if (Attacked < 1 && Attacked != -1) {
                            if (PlayerRight.ChiAvailable >= 2) {
                                PlayerAttack = new Task(PlayerRight.Attack, true);
                                PlayerAttack.Start();

                                if (Together == 1 && Player.PlayerDied == null) {
                                    PlayerLeft.ReceiveDamage(3, true);

                                    for (int i = 0; i < 5; i++) {
                                        PlayerLeft.Move(false);
                                    }

                                    Attacked = 0;

                                    continue;
                                }
                            }
                        }
                    }

                    int Aux = X.Next(0, 4);

                    Attacked = 0;

                    for (int i = 0; i < Aux; i++) {

                        lock (SyncRing) {

                            if (Player.PlayerDied == null) {
                                PlayerAttack = new Task(PlayerRight.Attack, false);
                                PlayerAttack.Start();

                                if (Together == 1 && !PlayerLeft.DefendingState) {
                                    PlayerLeft.ReceiveDamage(1);
                                    Attacked++;
                                } else {
                                    Attacked--;
                                }

                                PlayerAttack.Wait();
                            }

                        }
                        Thread.Sleep((Aux * 10) / 2);
                    }

                    lock (SyncRing)
                        PlayerRight.Defend();

                    Thread.Sleep(X.Next(70, 200));

                    Aux = X.Next(0, 10);

                    for (int i = 0; i < Aux; i++) {
                        lock (SyncRing)
                            Interlocked.Exchange(ref Together, PlayerRight.Move(true));
                        Thread.Sleep(100);
                    }
                }

                lock (SyncRing) {
                    if (Write.IsAtEdge(PlayerRight.Position) && Player.PlayerDied == null) {
                        PlayerRight.ReceiveDamage(1);
                    }

                }

            }
        }

        public void StartPlayer() {
            Task PlayerAttack;
            ConsoleKey Key;

            while (Player.PlayerDied == null) {

                Key = Console.ReadKey(true).Key;

                lock (SyncRing) {
                    if (Key == ConsoleKey.D && Player.PlayerDied == null) {

                        Interlocked.Exchange(ref Together, PlayerLeft.Move(true));
                    } else if (Key == ConsoleKey.A && Player.PlayerDied == null) {

                        Interlocked.Exchange(ref Together, PlayerLeft.Move(false));
                    } else if (Key == ConsoleKey.Enter && Player.PlayerDied == null) {

                        PlayerAttack = new Task(PlayerLeft.Attack, false);
                        PlayerAttack.Start();

                        if (Together == 1 && !PlayerRight.DefendingState)
                            PlayerRight.ReceiveDamage(1);

                        PlayerAttack.Wait();

                    } else if (Key == ConsoleKey.Spacebar && Player.PlayerDied == null) {
                        PlayerLeft.Defend();
                    } else if (Key == ConsoleKey.P && Player.PlayerDied == null) {

                        if (PlayerLeft.ChiAvailable >= 2) {
                            PlayerAttack = new Task(PlayerLeft.Attack, true);
                            PlayerAttack.Start();

                            if (Together == 1 && !PlayerRight.DefendingState) {

                                PlayerRight.ReceiveDamage(3, true);

                                for (int i = 0; i < 5; i++) {
                                    PlayerRight.Move(true);
                                }
                            }

                            PlayerAttack.Wait();
                        }
                    }

                    if (Write.IsAtEdge(PlayerLeft.Position) && Player.PlayerDied == null)
                        PlayerLeft.ReceiveDamage(1);
                }
            }
        }

        public void StartFight() {

            int Op;

            while (true) {

                Op = Write.Menu();

                Console.Clear();

                if (Op == 1) {

                    Write.RestartValues();

                    NewWidth = 119;

                    Write.WriteRing(10, 1);

                    PlayerRight.RestartPlayer(102, 40, true);
                    PlayerLeft.RestartPlayer(15, 40, true);

                    ResizeArena = new Thread(DecreaseArena);
                    IA = new Thread(StartIA);
                    Gamer = new Thread(StartPlayer);
                    ResizeArena.Start();

                    Gamer.Start();
                    IA.Start();

                    Gamer.Join();
                    IA.Join();
                    ResizeArena.Join();

                    Write.DeadCharacter(Player.PlayerDied.Side);

                    ConsoleKey Wait;

                    do {
                        Wait = Console.ReadKey(true).Key;
                    } while (Wait != ConsoleKey.Escape);

                    PlayerRight.RestartPlayer(102, 40);
                    PlayerLeft.RestartPlayer(15, 40);
                } else if (Op == 5) {
                    break;
                } else
                    continue;
            }
        }
    }
}