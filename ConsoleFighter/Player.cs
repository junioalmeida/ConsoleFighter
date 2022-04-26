using System;
using System.Threading;

namespace ConsoleFighter {
    public class Player {

        public static Player PlayerDied { get; private set; }

        private readonly object SyncAtkDefMove = new object();

        public int HealthPoints { get; private set; }
        private int _Position;

        public int Position {
            get {
                lock (SyncAtkDefMove)
                    return _Position;
            }
            private set { _Position = value; }
        }

        public bool DefendingState { get; private set; }
        public bool IsBeingAttacking { get; private set; }
        public int ChiAvailable { get; set; }

        public PlayerSide Side { get; private set; }

        private readonly IWritter Write;

        private Thread Chi;

        public Player(int initialPosition, PlayerSide Side, int HealthPoints) {
            Write = Writter.GetInstance();

            this.HealthPoints = HealthPoints;

            Position = initialPosition;
            DefendingState = false;
            this.Side = Side;
            ChiAvailable = 1;
        }

        public void RestartPlayer(int InitialPosition, int HealthPoints, bool RestartChi = false) {

            lock (SyncAtkDefMove) {
                if (Chi != null && Chi.IsAlive) {
                    Chi.Abort();
                }
            }

            PlayerDied = null;

            Position = InitialPosition;
            DefendingState = false;
            ChiAvailable = 1;
            this.HealthPoints = HealthPoints;

            Write.StayNormal(ref _Position, Side);

            if (RestartChi) {
                Chi = new Thread(GenerateChi) {
                    Name = "GenerateChi" + Side.ToString()
                };

                Chi.Start();
            }
        }

        public int Move(bool ToRight) {
            int toghter;

            while (true) {

                lock (SyncAtkDefMove) {
                    if (!IsBeingAttacking) {
                        if (ToRight)
                            Position++;
                        else
                            Position--;

                        if (DefendingState) {
                            toghter = Write.StayNormal(ref _Position, Side, ConsoleColor.Black, true);
                            DefendingState = false;
                        } else
                            toghter = Write.StayNormal(ref _Position, Side);

                        return toghter;
                    }
                }
            }
        }

        private void GenerateChi() {

            while (PlayerDied == null) {

                Thread.Sleep(3000);

                if (PlayerDied == null) {
                    lock (SyncAtkDefMove) {
                        if (ChiAvailable < 5) {
                            Write.ChiPoint(ChiAvailable + 1, Side);
                            ChiAvailable++;
                        } else {
                            Monitor.Wait(SyncAtkDefMove);
                        }
                    }
                }
            }
        }

        public void Attack(object IsEspecial) {

            for (int i = 0; i < 1; i++) {
                lock (SyncAtkDefMove) {

                    if ((bool)IsEspecial) {
                        if (ChiAvailable >= 2) {
                            ChiAvailable -= 2;
                            Write.ChiPoint(ChiAvailable, Side);
                        } else
                            break;

                        Monitor.PulseAll(SyncAtkDefMove);
                    }

                    DefendingState = false;

                    Write.Attack(_Position, Side);

                    Thread.Sleep(100);

                    Write.StayNormal(ref _Position, Side, ConsoleColor.Black, true);
                }
            }
        }

        public void Defend() {
            lock (SyncAtkDefMove) {

                if (!DefendingState) {

                    Write.Defend(_Position, Side);

                    DefendingState = true;
                }
            }

        }
                                                 
        public void ReceiveDamage(int AmountDamege, bool Especial = false) {

            lock (SyncAtkDefMove) {
                IsBeingAttacking = true;

                HealthPoints -= AmountDamege;
                Write.ReceiveDamage(HealthPoints, Side, ref _Position, Especial);

                if (HealthPoints <= 0) {
                    PlayerDied = this;
                }

                IsBeingAttacking = false;
            }
        }
    }
}
